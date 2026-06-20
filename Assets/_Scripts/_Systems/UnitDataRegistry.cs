using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Registro central de todos os UnitData "de batalha" (os assets usados de fato
/// na BattleScene, não os _VIEW da PreparationScene).
///
/// Resolve a correlação View -> Battle automaticamente pelo campo `unitName`,
/// sem depender do nome do arquivo do asset.
///
/// Este ScriptableObject deve viver em Assets/Resources/ (qualquer subpasta)
/// para que RuntimeInstance consiga carregá-lo em build via Resources.Load.
/// </summary>
[CreateAssetMenu(menuName = "Game/Unit Data Registry")]
public class UnitDataRegistry : ScriptableObject
{
    private const string ResourcesPath = "UnitDataRegistry";

    [Tooltip("Lista de todos os UnitData de batalha (os assets reais usados na BattleScene).")]
    public List<UnitData> battleUnits = new();

    private Dictionary<string, UnitData> lookup;
    private static UnitDataRegistry runtimeInstance;

    public static UnitDataRegistry Instance
    {
        get
        {
            if (runtimeInstance == null)
                runtimeInstance = Resources.Load<UnitDataRegistry>(ResourcesPath);

            return runtimeInstance;
        }
    }

    /// <summary>
    /// Dado um UnitData qualquer (seja ele o "view" da Preparation ou já o de batalha),
    /// retorna o UnitData de batalha correspondente, casando pelo campo `unitName`.
    /// Se não encontrar correspondência, retorna o próprio UnitData recebido (fallback seguro).
    /// </summary>
    public UnitData ResolveBattleUnit(UnitData source)
    {
        if (source == null)
            return null;

        BuildLookupIfNeeded();

        if (lookup.TryGetValue(source.unitName, out UnitData battleUnit))
            return battleUnit;

        Debug.LogWarning($"[UnitDataRegistry] Nenhum UnitData de batalha encontrado para '{source.unitName}'. Usando o asset original como fallback.");
        return source;
    }

    void BuildLookupIfNeeded()
    {
        if (lookup != null)
            return;

        lookup = new Dictionary<string, UnitData>();

        foreach (UnitData unit in battleUnits)
        {
            if (unit == null || string.IsNullOrEmpty(unit.unitName))
                continue;

            if (!lookup.ContainsKey(unit.unitName))
                lookup.Add(unit.unitName, unit);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Auto-Populate From Project")]
    void AutoPopulate()
    {
        battleUnits.Clear();
        lookup = null;

        string[] guids = AssetDatabase.FindAssets("t:UnitData");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // Ignora qualquer UnitData dentro de uma pasta "Units_view" (ou similar),
            // mantendo só os assets de batalha "reais".
            if (path.Contains("_view") || path.Contains("_VIEW") || path.Contains("View"))
                continue;

            UnitData unit = AssetDatabase.LoadAssetAtPath<UnitData>(path);
            if (unit != null)
                battleUnits.Add(unit);
        }

        EditorUtility.SetDirty(this);
        Debug.Log($"[UnitDataRegistry] Auto-populado com {battleUnits.Count} unidades de batalha.");
    }
#endif
}
