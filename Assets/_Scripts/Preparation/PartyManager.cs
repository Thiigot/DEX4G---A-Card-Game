using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public static PartyManager Instance;

    public List<UnitData> party = new();

    void Awake()
    {
        Instance = this;
    }

    public void Swap(int a, int b)
    {
        (party[a], party[b]) = (party[b], party[a]);
    }

    public void Remove(UnitData unit)
    {
        party.Remove(unit);
    }

    public void Add(UnitData unit)
    {
        if (party.Count >= 4) return;

        party.Add(unit);
    }
}