using CardData;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text deckCountText;
    public TMP_Text discardCountText;
    public TMP_Text handCountText;

    private List<GameObject> discardPileList = new List<GameObject>();
    public List<Card> deck = new List<Card>();
    public Transform deckPoint;
    public Transform discardPile;
    public int startingHandSize = 5;
    [SerializeField]private HandManager handManager;
    [SerializeField] private WarningUI warningUI;

    void Start()
    {
        handManager = FindAnyObjectByType<HandManager>();
        
        LoadDeck();
        ShuffleDeck();
        DrawStartingHand();
        UpdateUI();
    }

    void LoadDeck()
    {
        deck.Clear();
        Card[] cards = Resources.LoadAll<Card>("Outlaw");
        deck.AddRange(cards);
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int rand = Random.Range(i, deck.Count);
            (deck[i], deck[rand]) = (deck[rand], deck[i]);
        }
    }

    void DrawStartingHand()
    {
        for (int i = 0; i < startingHandSize; i++)
            DrawCard();
    }

    public void DrawCard()
    {
        


        if (handManager.transform.childCount >= handManager.maxHandSize)
        {
            if (warningUI != null)
                warningUI.Show("Mão cheia!");
            handManager.ShakeHand();
            return;
        }

        if (deck.Count == 0) return;

        Card card = deck[0];
        deck.RemoveAt(0);

        GameObject obj = Instantiate(handManager.cardPrefab, deckPoint.position, Quaternion.identity);
        handManager.AddCardToHand(obj);

        CardDisplay display = obj.GetComponent<CardDisplay>();
        display.cardData = card;
        display.UpdateCardDisplay();

        CardMovement move = obj.GetComponent<CardMovement>();
        move.SetHandManager(handManager);
        UpdateUI();
    }

    public void DiscardCard(GameObject card)
    {
        discardPileList.Add(card);

        card.transform.SetParent(discardPile);
        card.transform.localPosition = Vector3.zero;

        card.SetActive(false);

        UpdateUI();
    }

    void UpdateUI()
    {
        if (deckCountText != null)
            deckCountText.text = $"Cards in Deck: {deck.Count}";

        if (discardCountText != null)
            discardCountText.text = $"Cards in Discard Pile: {discardPileList.Count}";

        if (handCountText != null)
            handCountText.text = $"Cards in hand: {handManager.transform.childCount} / {handManager.maxHandSize}";
    }
}