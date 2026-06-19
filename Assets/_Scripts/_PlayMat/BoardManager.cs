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

    public void CompactBoard(bool playerSide)
    {
        List<BoardSlot> sideSlots = playerSide ? playerSlots : enemySlots;

        List<Unit> units = new();

        foreach (BoardSlot slot in sideSlots)
        {
            if (slot.currentUnit != null)
            {
                units.Add(slot.currentUnit);
            }
        }

        foreach (BoardSlot slot in sideSlots)
        {
            slot.Clear();
        }

        for (int i = 0; i < units.Count; i++)
        {
            sideSlots[i].SetUnit(units[i]);

            units[i].transform.position =
                sideSlots[i].transform.position;
        }
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
        if (unit == null || steps <= 0)
            return false;

        List<BoardSlot> sideSlots =
            unit.isPlayer ? playerSlots : enemySlots;

        if (sideSlots == null || sideSlots.Count == 0)
            return false;

        int currentIndex = sideSlots.IndexOf(unit.CurrentSlot);

        if (currentIndex < 0)
            return false;

        int direction = forward ? -1 : 1;

        int targetIndex = Mathf.Clamp(
            currentIndex + (direction * steps),
            0,
            sideSlots.Count - 1
        );

        if (targetIndex == currentIndex)
            return false;

        // Guarda a unidade que será movida
        Unit movingUnit = unit;

        // Movimento para frente
        if (targetIndex < currentIndex)
        {
            for (int i = currentIndex; i > targetIndex; i--)
            {
                Unit pushedUnit = sideSlots[i - 1].currentUnit;

                sideSlots[i].SetUnit(pushedUnit);

                if (pushedUnit != null)
                    pushedUnit.transform.position =
                        sideSlots[i].transform.position;
            }
        }
        // Movimento para trás
        else
        {
            for (int i = currentIndex; i < targetIndex; i++)
            {
                Unit pushedUnit = sideSlots[i + 1].currentUnit;

                sideSlots[i].SetUnit(pushedUnit);

                if (pushedUnit != null)
                    pushedUnit.transform.position =
                        sideSlots[i].transform.position;
            }
        }

        sideSlots[targetIndex].SetUnit(movingUnit);
        movingUnit.transform.position =
            sideSlots[targetIndex].transform.position;

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