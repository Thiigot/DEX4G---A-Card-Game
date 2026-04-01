using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [Header("Card Prefab")]
    public List<Transform> cards = new List<Transform>();
    public GameObject cardPrefab;
    private ManaManagerSTS manaManager;

    [Header("Layout")]
    public float spacing = 170f;
    public float curveHeight = 50f;
    public float maxAngle = 10f;
    public float moveSpeed = 10f;

    [Header("Hover")]
    public float hoverPushAmount = 100f;
    public CardMovement hoveredCard;

    [Header("Drag")]
    public CardMovement draggedCard;

    [Header("Hand Area")]
    public float handYThreshold = 100f;

    [Header("Limits")]
    public int maxHandSize = 10;

    void Start()
    {
        manaManager = FindAnyObjectByType<ManaManagerSTS>();
    }
    void LateUpdate()
    {
        UpdateCardsList();
        AnimateHand();
        UpdateManaVisuals();
    }

    public void UpdateCardsList()
    {
        cards.Clear();

        foreach (Transform child in transform)
        {
            if (child.GetComponent<CardMovement>() != null)
                cards.Add(child);
        }
    }

    void AnimateHand()
    {
        int count = cards.Count;
        if (count == 0) return;

        for (int i = 0; i < count; i++)
        {
            Transform card = cards[i];
            CardMovement movement = card.GetComponent<CardMovement>();

            if (movement == draggedCard)
                continue;

            float t = (count == 1) ? 0 : (i / (float)(count - 1)) * 2f - 1f;

            float x = t * spacing * (count - 1) / 2f;
            float y = -Mathf.Pow(t, 2) * curveHeight;

            Vector3 targetPos = new Vector3(x, y, 0);

            // 🔥 HOVER PUSH
            if (hoveredCard != null && movement != hoveredCard)
            {
                int hoveredIndex = cards.IndexOf(hoveredCard.transform);
                int currentIndex = i;

                float push = hoverPushAmount;

                if (currentIndex < hoveredIndex)
                    targetPos += Vector3.left * push;
                else if (currentIndex > hoveredIndex)
                    targetPos += Vector3.right * push;
            }

            movement.SetBasePosition(targetPos);

            float angle = -t * maxAngle;
            Quaternion rot = Quaternion.Euler(0, 0, angle);

            card.localRotation = Quaternion.Lerp(
                card.localRotation,
                rot,
                Time.deltaTime * moveSpeed
            );
        }
    }

    public bool IsCardInHandZone(CardMovement card)
    {
        if (card == null) return false;

        RectTransform handRect = transform as RectTransform;

        Vector3 localPos = handRect.InverseTransformPoint(card.transform.position);

        return localPos.y < handYThreshold;
    }

    public bool AddCardToHand(GameObject card)
    {
        if (cards.Count >= maxHandSize)
            return false;

        card.transform.SetParent(transform, false);

        CardMovement move = card.GetComponent<CardMovement>();
        move.SetHandManager(this);
        return true;
    }

    public void ShakeHand()
    {
        foreach (Transform card in transform)
        {
            CardMovement move = card.GetComponent<CardMovement>();
            if (move != null)
                move.TriggerShake();
        }
    }

    void UpdateManaVisuals()
    {
        if (manaManager == null) return;

        foreach (Transform card in transform)
        {
            CardDisplay display = card.GetComponent<CardDisplay>();
            if (display == null) continue;

            int cost = display.cardData.cardMana;
            bool canPlay = manaManager.currentMana >= cost;

            display.UpdateManaVisual(canPlay);
        }
    }
}