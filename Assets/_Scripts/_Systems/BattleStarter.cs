using System.Collections.Generic;
using UnityEngine;

public class BattleStarter : MonoBehaviour
{
    public BoardSlot[] allySlots;
    public BoardSlot[] enemySlots;

    public GameObject unitPrefab;

    [Header("Data Pools")]
    public UnitData[] enemyPool;

    [Header("Fallback (usado apenas se a BattleScene for aberta direto, sem passar pela PreparationScene)")]
    public PlayerPartyEntry[] playerPartyFallback;

    [System.Serializable]
    public class PlayerPartyEntry
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
        if (BattlePartyData.HasData)
        {
            SpawnAlliesFromBattlePartyData();
        }
        else
        {
            Debug.LogWarning("[BattleStarter] BattlePartyData vazio — usando playerPartyFallback do Inspector (modo de teste isolado).");
            SpawnAlliesFromFallback();
        }
    }

    void SpawnAlliesFromBattlePartyData()
    {
        // IMPORTANTE: a ordem de índices é invertida entre as duas cenas.
        // Na PreparationScene: índice 0 = backline mais distante, índice (Count-1) = frontline mais avançada.
        // Na BattleScene: allySlots[0] = frontline mais avançada, allySlots[Length-1] = backline mais distante.
        // Por isso percorremos compactedParty de trás para frente ao preencher allySlots a partir do índice 0.
        List<UnitData> compactedParty = BattlePartyData.CompactedParty;

        int slotIndex = 0;

        for (int i = compactedParty.Count - 1; i >= 0 && slotIndex < allySlots.Length; i--)
        {
            UnitData unitData = compactedParty[i];

            if (unitData != null)
            {
                // O UnitData vindo da Preparation Scene é o asset "_VIEW" (usado só para
                // exibição no draft). Resolvemos para o UnitData de batalha equivalente
                // (mesmas stats reais de combate) casando pelo unitName.
                UnitData battleUnitData = ResolveBattleUnitData(unitData);

                GameObject obj = Instantiate(unitPrefab);
                Unit unit = obj.GetComponent<Unit>();

                unit.Init(battleUnitData);
                allySlots[slotIndex].SetUnit(unit);
            }

            slotIndex++;
        }
    }

    UnitData ResolveBattleUnitData(UnitData source)
    {
        if (UnitDataRegistry.Instance == null)
        {
            Debug.LogWarning("[BattleStarter] UnitDataRegistry não encontrado em Resources. Usando o UnitData original sem resolver.");
            return source;
        }

        return UnitDataRegistry.Instance.ResolveBattleUnit(source);
    }

    void SpawnAlliesFromFallback()
    {
        foreach (var member in playerPartyFallback)
        {
            if (member.slotIndex >= allySlots.Length) continue;

            GameObject obj = Instantiate(unitPrefab);
            Unit unit = obj.GetComponent<Unit>();

            unit.Init(member.unitData);
            allySlots[member.slotIndex].SetUnit(unit);
        }
    }
}