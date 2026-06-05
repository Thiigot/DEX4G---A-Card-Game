using System.Collections.Generic;
using CardData;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class PartyDeckState
{
    public const int MaxDeckSize = 10;

    private static readonly Dictionary<UnitData, List<Card>> savedDecksByUnit = new();
    private static readonly Dictionary<UnitData, List<Card>> draftDecksByUnit = new();

    public static List<Card> GetDeck(UnitData unit)
    {
        return GetSavedDeck(unit);
    }

    public static List<Card> GetSavedDeck(UnitData unit)
    {
        if (unit == null)
            return new List<Card>();

        if (!savedDecksByUnit.TryGetValue(unit, out List<Card> deck))
        {
            deck = new List<Card>(LoadSavedDeck(unit));
            TrimToMaxSize(deck);
            savedDecksByUnit.Add(unit, deck);
        }

        return deck;
    }

    public static List<Card> BeginDraft(UnitData unit)
    {
        if (unit == null)
            return new List<Card>();

        List<Card> draft = new List<Card>(GetSavedDeck(unit));
        draftDecksByUnit[unit] = draft;
        return draft;
    }

    public static List<Card> GetDraft(UnitData unit)
    {
        if (unit == null)
            return new List<Card>();

        if (!draftDecksByUnit.TryGetValue(unit, out List<Card> draft))
            draft = BeginDraft(unit);

        return draft;
    }

    public static bool TryAddToDraft(UnitData unit, Card card)
    {
        if (unit == null || card == null)
            return false;

        List<Card> draft = GetDraft(unit);

        if (draft.Count >= MaxDeckSize)
            return false;

        if (draft.Contains(card))
            return false;

        draft.Add(card);
        return true;
    }

    public static bool RemoveFromDraft(UnitData unit, Card card)
    {
        if (unit == null || card == null)
            return false;

        return GetDraft(unit).Remove(card);
    }

    public static void SaveDraft(UnitData unit)
    {
        if (unit == null)
            return;

        List<Card> savedDeck = new List<Card>(GetDraft(unit));
        TrimToMaxSize(savedDeck);

        savedDecksByUnit[unit] = savedDeck;
        SaveDeckAsset(unit, savedDeck);
        draftDecksByUnit.Remove(unit);
    }

    public static void DiscardDraft(UnitData unit)
    {
        if (unit == null)
            return;

        draftDecksByUnit.Remove(unit);
    }

    public static bool IsInDraft(UnitData unit, Card card)
    {
        if (unit == null || card == null)
            return false;

        return GetDraft(unit).Contains(card);
    }

    public static IReadOnlyList<Card> GetAvailableCards(UnitData unit)
    {
        if (unit == null || unit.deckData == null || unit.deckData.availableCards == null)
            return new List<Card>();

        if (unit.deckData.availableCards.Count == 0 && unit.deckData.startingDeck != null)
            return unit.deckData.startingDeck;

        return unit.deckData.availableCards;
    }

    static IEnumerable<Card> LoadSavedDeck(UnitData unit)
    {
        if (unit == null || unit.deckData == null || unit.deckData.startingDeck == null)
            return new List<Card>();

        return unit.deckData.startingDeck;
    }

    static void TrimToMaxSize(List<Card> deck)
    {
        if (deck.Count > MaxDeckSize)
            deck.RemoveRange(MaxDeckSize, deck.Count - MaxDeckSize);
    }

    static void SaveDeckAsset(UnitData unit, List<Card> savedDeck)
    {
        if (unit == null || unit.deckData == null)
            return;

        unit.deckData.startingDeck.Clear();
        unit.deckData.startingDeck.AddRange(savedDeck);

#if UNITY_EDITOR
        EditorUtility.SetDirty(unit.deckData);
        AssetDatabase.SaveAssets();
#endif
    }
}
