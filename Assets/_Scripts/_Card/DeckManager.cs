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
    public List<Card> exile = new List<Card>();

    public Transform deckPoint;
    public Transform discardPile;
    DeckUIManager deckUI;
    public int startingHandSize = 5;
    [SerializeField] private HandManager handManager;
    [SerializeField] private WarningUI warningUI;
    [SerializeField] private Card zenBladeCard;

    public Action OnDeckChanged;
    private void Awake()
    {
        deckUI = FindAnyObjectByType<DeckUIManager>();
    }
    public void Init(Unit owner, HandManager hand)
    {
        this.handManager = hand;
        this.warningUI = FindAnyObjectByType<WarningUI>();
        deckUI = FindAnyObjectByType<DeckUIManager>();
        LoadDeck(owner);
        ShuffleDeck();

        ResetTemporaryCosts();

        OnDeckChanged?.Invoke();
    }
    void LoadDeck(Unit owner)
    {
        deck.Clear();

        List<Card> cards = owner != null && owner.data != null
            ? PartyDeckState.GetDeck(owner.data)
            : new List<Card>();

        foreach (var c in cards)
        {
            if (c != null)
                deck.Add(c);
            else
                Debug.LogError("Carta NULL encontrada no deck de " + owner.unitName);
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
        //if (TargetManager.isTargeting)
        //{
        //    if (warningUI != null)
        //        warningUI.Show("Select Target!");
        //    handManager.ShakeHand();
        //    return null;
        //}

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
    public void Reshuffle()
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


    public void BanishCard(Card card, Unit owner)
    {
        deck.Remove(card);
        discard.Remove(card);
        owner.hand.Remove(card);

        exile.Add(card);
        owner.handManager.RemoveCardVisual(card);
    }

    public void AddCardToDeck(Card card)
    {
        if (card == null)
            return;

        deck.Add(card);
        ShuffleDeck();
        deckUI.UpdateUI();
    }

    public void ZenBladeGen(int amount, Unit caster)
    {
        for (int i = 0; i < amount; i++)
        {
            caster.deckManager.AddCardToDeck(zenBladeCard);
        }
    }
    public Card PeekTopCard()
    {
        if (deck.Count == 0)
            return null;

        return deck[0];
    }
    public Card DrawTopCard()
    {
        if (deck.Count == 0)
            return null;

        Card card = deck[0];
        deck.RemoveAt(0);

        return card;
    }

    public void ResetTemporaryCosts()
    {

        foreach (Card card in deck)
        {
            if (card == null)
            {
                continue;
            }

            card.ClearTemporaryCost();
        }

        foreach (Card card in discard)
        {
            if (card == null)
            {
                continue;
            }

            card.ClearTemporaryCost();
        }

        foreach (Card card in exile)
        {
            if (card == null)
            {
                continue;
            }

            card.ClearTemporaryCost();
        }

        if (handManager != null &&
            handManager.owner != null &&
            handManager.owner.hand != null)
        {
            for (int i = handManager.owner.hand.Count - 1; i >= 0; i--)
            {
                Card card = handManager.owner.hand[i];

                if (card == null)
                {
                    handManager.owner.hand.RemoveAt(i);
                    continue;
                }

                card.ClearTemporaryCost();
            }
        }

        deckUI?.UpdateUI();
    }
}
