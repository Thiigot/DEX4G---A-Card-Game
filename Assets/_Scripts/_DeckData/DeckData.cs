using System.Collections.Generic;
using CardData;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Deck Data")]
public class DeckData : ScriptableObject
{
    public string deckName;
    public CardClass cardClass;
    [Header("Cards available in deckbuilder")]
    public List<Card> availableCards = new();

    [Header("Saved deck used by this unit")]
    public List<Card> startingDeck = new();
}