using CardData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Captain/Bombing Raid")]
public class BombingRaidEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        if (caster.IsFrontline())
        {
            List<Unit> enemies = CardEffectExecutor.GetAllEnemies(caster);
            foreach(Unit enemy in enemies)
            {
                enemy.TakeDamage(caster.ModifyOutgoingDamage(2), DamageType.Direct, caster);
                TryApplyStun(enemy);
                yield return new WaitForSeconds(0.15f);
            }
        }
        else
        {
            Unit lastEnemy = GetBackmostEnemy(caster);
            if (lastEnemy == null) yield break;
            lastEnemy.TakeDamage(caster.ModifyOutgoingDamage(2), DamageType.Direct, caster);
            TryApplyStun(lastEnemy);
            yield break;
        }
        yield break;
    }

    private void TryApplyStun(Unit target)
    {
        if(Random.Range(0,100) < 40)
        {
            target.AddStatus(new StunEffect { value = 1 });
        }
    }
    private Unit GetBackmostEnemy(Unit caster)
    {
        List<BoardSlot> enemySlots = caster.isPlayer ? BoardManager.Instance.enemySlots : BoardManager.Instance.playerSlots;
        for(int i = enemySlots.Count - 1; i >= 0; i--)
        {
            if (enemySlots[i].currentUnit != null) return enemySlots[i].currentUnit;
        }
        return null;
    }
}