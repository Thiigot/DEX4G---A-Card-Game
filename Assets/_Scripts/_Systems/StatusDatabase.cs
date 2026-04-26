using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Status Database")]
public class StatusDatabase : ScriptableObject
{
    public List<StatusEntry> entries;

    private Dictionary<StatusType, Sprite> lookup;

    void OnEnable()
    {
        BuildDictionary();
    }

    void BuildDictionary()
    {
        lookup = new Dictionary<StatusType, Sprite>();

        foreach (var entry in entries)
        {
            if (!lookup.ContainsKey(entry.type))
                lookup.Add(entry.type, entry.icon);
        }
    }

    public Sprite GetIcon(StatusType type)
    {
        if (lookup == null || lookup.Count == 0)
            BuildDictionary();

        if (lookup.TryGetValue(type, out var icon))
            return icon;

        Debug.LogWarning("Ícone não encontrado para: " + type);
        return null;
    }
}

[System.Serializable]
public class StatusEntry
{
    public StatusType type;
    public Sprite icon;
}