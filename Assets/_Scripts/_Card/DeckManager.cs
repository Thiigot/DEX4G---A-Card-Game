using CardData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeckManager : MonoBehaviour
{
    public List<Card> discard = new List<Card>();
    public List<Card> deck = new List<Card>();
    public Transform deckPoint;
    public Transform discardPile;
    DeckUIManager deckUI;
    public int startingHandSize = 5;

    [SerializeField]private HandManager handManager;
    [SerializeField] private WarningUI warningUI;

    public Action OnDeckChanged;
    private void Awake()
    {
        deckUI = FindAnyObjectByType<DeckUIManager>();
    }
    public void Init(Unit owner, HandManager hand)
    {
        this.handManager = hand;
        this.warningUI = FindAnyObjectByType<WarningUI>();
        LoadDeck(owner);
        ShuffleDeck();

        OnDeckChanged?.Invoke();
    }
    void LoadDeck(Unit owner)
    {
        string path = owner.unitClass;
        Card[] cards = Resources.LoadAll<Card>(path);
        foreach (var c in cards)
        {
            if (c != null)
                deck.Add(c);
            else
                Debug.LogError("Carta NULL encontrada em Resources/" + path);
        }
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int rand = Random.Range(i, deck.Count);
            (deck[i], deck[rand]) = (deck[rand], deck[i]);
        }
    }

    public Card DrawCard()
    {
        if (TargetManager.isTargeting)
        {
            if (warningUI != null)
                warningUI.Show("Select Target!");
            handManager.ShakeHand();
            return null;
        }

        if (handManager.owner.hand.Count >= handManager.maxHandSize)
        {
            if (warningUI != null)
                warningUI.Show("Hand full!");
            handManager.ShakeHand();
            return null;
        }

        if (deck.Count == 0)
        {
            Reshuffle();
        }

        Card card = deck[0];
        deck.RemoveAt(0);
        return card;
    }

    public int GetDeckCount() => deck.Count;
    public int GetDiscardCount() => discard.Count;
    public int GetHandCount() => handManager.transform.childCount;
    public Unit GetOwner() => handManager.owner;

    public void AddToDiscard(Card card)
    {
        discard.Add(card);
    }
    void Reshuffle()
    {
        deck.AddRange(discard);
        discard.Clear();

        ShuffleDeck();
    }

    public IEnumerator AnimateDiscard(GameObject cardObj)
    {
        if (cardObj == null) yield break;

        RectTransform rect = cardObj.GetComponent<RectTransform>();
        if (rect == null) yield break;

        Vector3 start = rect.position;
        Vector3 end = discardPile.position;

        float t = 0;
        float duration = 0.25f;

        while (t < 1)
        {
            if (rect == null) yield break; // 🔥 proteção
            float curve = Mathf.SmoothStep(0, 1, t); // 🔥 suaviza
            t += Time.deltaTime / duration;
            rect.position = Vector3.Lerp(start, end, curve);
            float scale = Mathf.Lerp(1f, 0.4f, curve);
            rect.localScale = Vector3.one * scale;
            yield return null;
        }

        if (cardObj != null)
            cardObj.SetActive(false);
    }
}