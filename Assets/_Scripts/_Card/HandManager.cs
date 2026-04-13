using CardData;
using System.Collections;
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

    public Unit owner;

    public void SetOwner(Unit unit)
    {
        owner = unit;
    }
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
            CardMovement cm = child.GetComponent<CardMovement>();

            if (cm != null && child.parent == transform) // 🔥 garantia extra
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
        if (owner == null) return;
        if (manaManager == null) return;

        foreach (Transform card in transform)
        {
            CardDisplay display = card.GetComponent<CardDisplay>();
            if (display == null) continue;
            if (display.cardData == null) continue;
            int cost = display.cardData.cardMana;
            bool canPlay = manaManager.currentMana >= cost;

            display.UpdateManaVisual(canPlay);
        }
    }

    public void ClearHandVisual()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ShowHand(List<Card> cards)
    {
        ClearHandVisual();

        foreach (var card in cards)
        {
            if (card == null)
            {
                continue;
            }
            GameObject obj = Instantiate(cardPrefab, transform);

            // posição inicial
            obj.transform.localPosition = Vector3.zero;

            // display
            CardDisplay display = obj.GetComponent<CardDisplay>();
            display.cardData = card;
            display.UpdateCardDisplay();

            // movimento
            CardMovement move = obj.GetComponent<CardMovement>();
            move.SetHandManager(this);
        }
    }

    public IEnumerator AnimateDrawCard(Card card, Transform deckPoint)
    {
        // 🔹 1. Instancia na mão (invisível) para calcular posição final
        GameObject obj = Instantiate(cardPrefab, transform);
        obj.SetActive(false);

        RectTransform rect = obj.GetComponent<RectTransform>();

        // setup visual
        CardDisplay display = obj.GetComponent<CardDisplay>();
        display.cardData = card;
        display.UpdateCardDisplay();

        CardMovement move = obj.GetComponent<CardMovement>();
        move.SetHandManager(this);

        // 🔹 2. Força layout atualizar
        Canvas.ForceUpdateCanvases();

        // 🔹 3. Pega posição FINAL REAL
        Vector3 targetPos = rect.position;

        // 🔹 4. Move carta para o DECK (fora da mão)
        obj.transform.SetParent(transform.root, true);
        rect.position = deckPoint.position;
        rect.localScale = Vector3.one * 0.6f;

        obj.SetActive(true);

        Vector3 start = deckPoint.position;

        float duration = 0.35f;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / duration;

            float curve = Mathf.SmoothStep(0, 1, t);

            // 🔥 movimento com leve arco
            Vector3 mid = (start + targetPos) / 2 + Vector3.up * 100f;

            rect.position =
                Mathf.Pow(1 - curve, 2) * start +
                2 * (1 - curve) * curve * mid +
                Mathf.Pow(curve, 2) * targetPos;

            rect.localScale = Vector3.Lerp(Vector3.one * 0.6f, Vector3.one, curve);

            yield return null;
        }

        // 🔹 5. Entra na mão (sem teleport)
        obj.transform.SetParent(transform, true);
    }

    public GameObject GetCardObject(Card card)
    {
        foreach (Transform child in transform)
        {
            CardDisplay display = child.GetComponent<CardDisplay>();

            if (display != null && display.cardData == card)
                return child.gameObject;
        }

        return null;
    }
}