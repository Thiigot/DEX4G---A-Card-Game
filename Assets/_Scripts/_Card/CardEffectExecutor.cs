using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardData;

public static class CardEffectExecutor
{

    //----------------------------------------
    // EXECUÇÃO COM COROUTINE
    //----------------------------------------
    public static IEnumerator ExecuteCardCoroutine(
    Unit caster,
    Unit target,
    Card card)
    {
        if (card == null)
            yield break;

        CardSpecialEffect effect = card.specialEffect;

        if (effect == null)
            yield break;

        yield return effect.OnPlayCoroutine(
            caster,
            target,
            card
        );
    }

    //----------------------------------------
    // SISTEMAS AUXILIARES
    //----------------------------------------

    public static Unit GetAutomaticTarget(
        Unit caster,
        Card card)
    {
        List<Unit> enemies = GetAllEnemies(caster);

        if (enemies.Count == 0)
            return null;

        return enemies[
            Random.Range(0, enemies.Count)
        ];
    }

    public static void MoveUnit(
        Unit unit,
        int steps,
        bool forward)
    {
        BoardManager boardManager = BoardManager.Instance;

        if (boardManager == null)
            boardManager = GameObject.FindAnyObjectByType<BoardManager>();

        if (boardManager == null)
        {
            Debug.LogWarning(
                "BoardManager não encontrado para mover unidade."
            );
            return;
        }

        boardManager.TryMoveUnit(
            unit,
            steps,
            forward
        );
    }

    public static List<Unit> GetAllEnemies(Unit caster)
    {
        List<Unit> result = new();

        foreach (var unit in GameObject.FindObjectsOfType<Unit>())
        {
            if (unit.isPlayer != caster.isPlayer)
                result.Add(unit);
        }

        return result;
    }

    public static List<Unit> GetAllAllies(Unit caster)
    {
        List<Unit> result = new();

        foreach (var unit in GameObject.FindObjectsOfType<Unit>())
        {
            if (unit.isPlayer == caster.isPlayer)
                result.Add(unit);
        }

        return result;
    }
}