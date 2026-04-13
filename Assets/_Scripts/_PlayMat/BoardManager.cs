using UnityEngine;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    public List<BoardSlot> playerSlots;
    public List<BoardSlot> enemySlots;
    public List<BoardSlot> allSlots;


    public List<Unit> GetAllEnemies(bool playerSide)
    {
        List<Unit> result = new List<Unit>();

        var list = playerSide ? enemySlots : playerSlots;

        foreach (var slot in list)
        {
            if (slot.currentUnit != null)
                result.Add(slot.currentUnit);
        }

        return result;
    }

    public BoardSlot GetRandomEnemySlot(bool playerSide)
    {
        var enemies = GetAllEnemies(playerSide);
        if (enemies.Count == 0) return null;

        return enemies[Random.Range(0, enemies.Count)].GetComponentInParent<BoardSlot>();
    }
}