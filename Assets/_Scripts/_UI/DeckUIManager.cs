using TMPro;
using UnityEngine;

public class DeckUIManager : MonoBehaviour
{
    public TMP_Text deckCountText;
    public TMP_Text discardCountText;
    public TMP_Text handCountText;
    public TMP_Text turnUnitText;

    private DeckManager currentDeck;
    private HandManager handManager;

    public Transform deckPoint;
    public Transform discardPile;
    public void SetCurrentDeck(DeckManager deck, HandManager hand)
    {
        // Desinscreve do anterior
        if (currentDeck != null)
            currentDeck.OnDeckChanged -= UpdateUI;

        currentDeck = deck;
        handManager = hand;

        currentDeck.OnDeckChanged += UpdateUI;

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (currentDeck == null) return;

        if (deckCountText != null)
            deckCountText.text = $"DECK: {currentDeck.deck.Count}";

        if (discardCountText != null)
            discardCountText.text = $"DISCARD: {currentDeck.discard.Count}";

        if (handCountText != null)
            handCountText.text = $"HAND: {handManager.owner.hand.Count} / {handManager.maxHandSize}";

        if (turnUnitText != null)
            turnUnitText.text = $"{currentDeck.GetOwner().unitClass}'s Turn";
    }
    public void UpdateTurnText(Unit unit)
    {
        if (turnUnitText == null) return;

        turnUnitText.text = $"{unit.unitName}'s Turn";
    }
}