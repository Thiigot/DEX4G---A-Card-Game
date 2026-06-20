using System.Collections.Generic;

/// <summary>
/// Carrega a formação final da party (já compactada pelas regras de ouro)
/// da PreparationScene para a BattleScene.
///
/// É uma classe estática simples (não um GameObject), então sobrevive
/// naturalmente à troca de cena sem precisar de DontDestroyOnLoad.
/// </summary>
public static class BattlePartyData
{
    /// <summary>
    /// Índice no array = slot na Battle Scene (0 = backline mais distante,
    /// Count-1 = frontline mais avançada). Pode conter nulls (slots vazios).
    /// </summary>
    public static List<UnitData> CompactedParty { get; private set; }

    public static bool HasData => CompactedParty != null;

    public static void SetParty(List<UnitData> compactedParty)
    {
        CompactedParty = compactedParty;
    }

    public static void Clear()
    {
        CompactedParty = null;
    }
}
