using System.Collections.Generic;
using UnityEngine;

public class BattleStarter : MonoBehaviour
{
    public BoardSlot[] allySlots;
    public BoardSlot[] enemySlots;

    public GameObject unitPrefab;

    [Header("Data Pools")]
    public UnitData[] enemyPool;
    public PartySlot[] playerParty;

    [System.Serializable]
    public class PartySlot
    {
        public UnitData unitData;
        public int slotIndex;
    }

    void Start()
    {
        SpawnEnemies();
        SpawnAllies();
    }

    void SpawnEnemies()
    {
        List<UnitData> pool = new List<UnitData>(enemyPool);

        foreach (var slot in enemySlots)
        {
            if (pool.Count == 0) break;

            int rand = Random.Range(0, pool.Count);
            UnitData enemy = pool[rand];
            pool.RemoveAt(rand);

            GameObject obj = Instantiate(unitPrefab);
            Unit unit = obj.GetComponent<Unit>();

            unit.Init(enemy);
            slot.SetUnit(unit);
        }
    }

    void SpawnAllies()
    {
        foreach (var member in playerParty)
        {
            if (member.slotIndex >= allySlots.Length) continue;

            GameObject obj = Instantiate(unitPrefab);
            Unit unit = obj.GetComponent<Unit>();

            unit.Init(member.unitData);
            allySlots[member.slotIndex].SetUnit(unit);
        }
    }
}