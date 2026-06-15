using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BoardManager : MonoBehaviour
{

    public static BoardManager Instance;

    public List<BoardSlot> playerSlots;
    public List<BoardSlot> enemySlots;
    public List<BoardSlot> allSlots;

    private void Awake()
    {
        Instance = this;
    }

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

    public bool TryMoveUnit(Unit unit, int steps, bool forward)
    {
        if (unit == null || steps <= 0) return false;

        List<BoardSlot> sideSlots = unit.isPlayer ? playerSlots : enemySlots;
        if (sideSlots == null || sideSlots.Count == 0) return false;

        int currentIndex = sideSlots.IndexOf(unit.CurrentSlot);
        if (currentIndex < 0) return false;

        int direction = forward ? -1 : 1;
        int targetIndex = Mathf.Clamp(currentIndex + direction * steps, 0, sideSlots.Count - 1);

        if (targetIndex == currentIndex) return false;

        BoardSlot currentSlot = sideSlots[currentIndex];
        BoardSlot targetSlot = sideSlots[targetIndex];

        Unit otherUnit = targetSlot.currentUnit;

        currentSlot.Clear();
        targetSlot.Clear();

        targetSlot.SetUnit(unit);
        unit.transform.position = targetSlot.transform.position;

        if (otherUnit != null)
        {
            currentSlot.SetUnit(otherUnit);
            otherUnit.transform.position = currentSlot.transform.position;
        }

        return true;
    }
    public BoardSlot GetFrontMostSlot(bool playerSide)
    {
        List<BoardSlot> slots = playerSide ? playerSlots : enemySlots;

        return slots[0];
    }


    public BoardSlot GetRandomEnemySlot(bool playerSide)
    {
        var enemies = GetAllEnemies(playerSide);
        if (enemies.Count == 0) return null;

        return enemies[Random.Range(0, enemies.Count)].GetComponentInParent<BoardSlot>();
    }
}