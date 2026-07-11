using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Salva e carrega a última formação de party (quais unidades, em quais slots)
/// usando PlayerPrefs + JSON. Funciona tanto no Editor quanto em build final,
/// sem depender de AssetDatabase ou qualquer API exclusiva de Editor.
///
/// Salva pelo `unitName` dos UnitData (assets _VIEW da Preparation), já que
/// nomes são estáveis e não dependem de referências de asset/GUID.
/// </summary>
public static class PartyPrefsManager
{
    private const string PrefsKey = "DEX4G_LastParty";

    [System.Serializable]
    private class PartySlotEntry
    {
        public string unitName;
        public int slotIndex;
    }

    [System.Serializable]
    private class PartySaveData
    {
        public List<PartySlotEntry> slots = new();
    }

    /// <summary>
    /// Salva o estado atual da party (lista de UnitData, podendo conter nulls).
    /// O índice na lista é preservado como o slotIndex.
    /// </summary>
    public static void SaveParty(List<UnitData> party)
    {
        PartySaveData data = new PartySaveData();

        for (int i = 0; i < party.Count; i++)
        {
            UnitData unit = party[i];
            if (unit == null) continue;

            data.slots.Add(new PartySlotEntry
            {
                unitName = unit.unitName,
                slotIndex = i
            });
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(PrefsKey, json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Carrega a última party salva, retornando uma lista do mesmo tamanho
    /// que `availableUnits` (roster completo), com cada UnitData posicionado
    /// no slotIndex salvo. Unidades não encontradas no roster são ignoradas.
    /// </summary>
    public static List<UnitData> LoadParty(int partySize, IEnumerable<UnitData> availableUnits)
    {
        List<UnitData> result = new(new UnitData[partySize]);

        if (!HasSavedParty())
            return result;

        string json = PlayerPrefs.GetString(PrefsKey);
        PartySaveData data;

        try
        {
            data = JsonUtility.FromJson<PartySaveData>(json);
        }
        catch
        {
            return result;
        }

        if (data == null || data.slots == null)
            return result;

        Dictionary<string, UnitData> byName = new();
        foreach (UnitData unit in availableUnits)
        {
            if (unit != null && !byName.ContainsKey(unit.unitName))
                byName.Add(unit.unitName, unit);
        }

        foreach (PartySlotEntry entry in data.slots)
        {
            if (entry.slotIndex < 0 || entry.slotIndex >= partySize)
                continue;

            if (byName.TryGetValue(entry.unitName, out UnitData unit))
                result[entry.slotIndex] = unit;
        }

        return result;
    }

    public static bool HasSavedParty()
    {
        return PlayerPrefs.HasKey(PrefsKey);
    }

    public static void ClearSavedParty()
    {
        PlayerPrefs.DeleteKey(PrefsKey);
    }
}
