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
    public float critChance;
}