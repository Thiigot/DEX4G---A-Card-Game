using System;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public static PartyManager Instance;

    public const int MaxPartySize = 4;

    [Header("Party Data")]
    public List<UnitData> party = new();
    public List<UnitData> roster = new();

    [Header("Scene References")]
    [SerializeField] private PartySlot[] partySlots;
    [SerializeField] private PartyCharacterView characterViewPrefab;

    public event Action OnPartyChanged;
    public event Action<UnitData> OnSelectedUnitChanged;
    public event Action<UnitData> OnDetailsUnitChanged;

    public UnitData SelectedUnit { get; private set; }
    public UnitData DetailsUnit { get; private set; }

    void Awake()
    {
        Instance = this;
        NormalizeParty();
    }

    void Start()
    {
        InitializeSlots();
        RebuildPartyViews();
    }

    public void Swap(int a, int b)
    {
        if (!IsValidSlot(a) || !IsValidSlot(b)) return;
        if (a == b)
        {
            RebuildPartyViews();
            return;
        }

        (party[a], party[b]) = (party[b], party[a]);
        RebuildPartyViews();
        if (SelectedUnit != null)
            Select(SelectedUnit);
    }

    public bool MoveToSlot(UnitData unit, int targetIndex)
    {
        if (unit == null || !IsValidSlot(targetIndex)) return false;

        int currentIndex = party.IndexOf(unit);

        if (currentIndex >= 0)
        {
            Swap(currentIndex, targetIndex);
            return true;
        }

        if (party[targetIndex] != null)
            return false;

        party[targetIndex] = unit;
        RebuildPartyViews();
        return true;
    }

    public void Remove(UnitData unit)
    {
        int index = party.IndexOf(unit);
        if (index < 0) return;

        RemoveAt(index);
    }

    public void RemoveAt(int index)
    {
        if (!IsValidSlot(index)) return;

        UnitData removed = party[index];
        party[index] = null;

        if (SelectedUnit == removed)
            Select(null);

        if (DetailsUnit == removed)
            CloseDetails();

        RebuildPartyViews();
    }

    public void Add(UnitData unit)
    {
        AddToFirstEmptySlot(unit);
    }

    public bool AddToFirstEmptySlot(UnitData unit)
    {
        if (unit == null || party.Contains(unit)) return false;

        int index = party.FindIndex(slotUnit => slotUnit == null);
        if (index < 0) return false;

        party[index] = unit;
        RebuildPartyViews();
        return true;
    }

    public void Select(UnitData unit)
    {
        SelectedUnit = unit;
        UpdateSelectedPartyViews();
        OnSelectedUnitChanged?.Invoke(unit);
    }

    public void OpenDetails(UnitData unit)
    {
        if (unit == null || !IsInParty(unit))
            return;

        SelectedUnit = unit;
        DetailsUnit = unit;
        UpdateSelectedPartyViews();
        OnSelectedUnitChanged?.Invoke(SelectedUnit);
        OnDetailsUnitChanged?.Invoke(DetailsUnit);
    }

    public void OpenSelectedDetails()
    {
        OpenDetails(SelectedUnit);
    }

    public void CloseDetails()
    {
        DetailsUnit = null;
        OnDetailsUnitChanged?.Invoke(null);
    }

    public UnitData GetUnitAtSlot(int slotIndex)
    {
        if (!IsValidSlot(slotIndex))
            return null;

        return party[slotIndex];
    }

    public void SelectSlot(int slotIndex)
    {
        Select(GetUnitAtSlot(slotIndex));
    }

    public PartyCharacterView GetSelectedCharacterView()
    {
        if (SelectedUnit == null || partySlots == null)
            return null;

        foreach (PartySlot slot in partySlots)
        {
            if (slot == null || slot.currentCharacter == null) continue;
            if (slot.currentCharacter.data == SelectedUnit)
                return slot.currentCharacter;
        }

        return null;
    }

    public bool IsInParty(UnitData unit)
    {
        return unit != null && party.Contains(unit);
    }

    void RebuildPartyViews()
    {
        InitializeSlots();

        if (SelectedUnit != null && !party.Contains(SelectedUnit))
            SelectedUnit = null;

        if (DetailsUnit != null && !party.Contains(DetailsUnit))
            DetailsUnit = null;

        foreach (PartySlot slot in partySlots)
            slot.ClearAndDestroyView();

        for (int i = 0; i < party.Count && i < partySlots.Length; i++)
        {
            UnitData unit = party[i];
            if (unit == null) continue;

            PartyCharacterView view = Instantiate(characterViewPrefab, partySlots[i].transform);
            view.Setup(unit, partySlots[i], false);
            partySlots[i].SetCharacter(view);
            view.SetSelectedVisual(unit == SelectedUnit);
        }

        NotifyPartyChanged();
        OnSelectedUnitChanged?.Invoke(SelectedUnit);
        OnDetailsUnitChanged?.Invoke(DetailsUnit);
    }

    void NotifyPartyChanged()
    {
        OnPartyChanged?.Invoke();
    }

    void InitializeSlots()
    {
        if (partySlots == null || partySlots.Length == 0)
            partySlots = FindObjectsByType<PartySlot>(FindObjectsSortMode.None);

        Array.Sort(partySlots, (a, b) => a.index.CompareTo(b.index));

        for (int i = 0; i < partySlots.Length; i++)
            partySlots[i].index = i;
    }

    void NormalizeParty()
    {
        while (party.Count < MaxPartySize)
            party.Add(null);

        if (party.Count > MaxPartySize)
            party.RemoveRange(MaxPartySize, party.Count - MaxPartySize);
    }

    bool IsValidSlot(int index)
    {
        return index >= 0 && index < party.Count;
    }

    void UpdateSelectedPartyViews()
    {
        if (partySlots == null) return;

        foreach (PartySlot slot in partySlots)
        {
            if (slot == null || slot.currentCharacter == null) continue;

            slot.currentCharacter.SetSelectedVisual(slot.currentCharacter.data == SelectedUnit);
        }
    }
}
