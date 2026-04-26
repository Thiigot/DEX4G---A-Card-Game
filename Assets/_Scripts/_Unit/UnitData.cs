using UnityEngine;

[CreateAssetMenu(menuName = "Game/Unit Data")]
public class UnitData : ScriptableObject
{
    public string unitName;
    public Sprite sprite;

    [Header("Stats")]
    public int maxHP;
    public int attack;
    public int speed;
    public int baseMana;

    [Header("Combat Stats")]
    public float critChance;
    public float dodgeChance;
    public float retaliateChance;

    public float protection; // %
    public float weakness;   // %

    [Header("Resources")]
    public int baseLuck;
    public int baseChip;

    [Header("Traits")]
    public bool isFrontline;
    public bool isBackline;

    [Header("Flags")]
    public bool isPlayer;
}