using System.Collections.Generic;
using CardData;
using UnityEngine;

public static class PartyDeckState
{
    private static readonly Dictionary<UnitData, List<Card>> decksByUnit = new();

    public static List<Card> GetDeck(UnitData unit)
    {
        if (unit == null)
            return new List<Card>();

        if (!decksByUnit.TryGetValue(unit, out List<Card> deck))
        {
            deck = new List<Card>(LoadDefaultDeck(unit));
            decksByUnit.Add(unit, deck);
        }

        return deck;
    }

    static IEnumerable<Card> LoadDefaultDeck(UnitData unit)
    {
        if (unit == null || string.IsNullOrWhiteSpace(unit.unitName))
            return new List<Card>();

        return Resources.LoadAll<Card>(unit.unitName);
    }
}
