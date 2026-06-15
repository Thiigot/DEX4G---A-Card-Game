using CardData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Captain/Naval Barrage")]
public class NavalBarrageEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        List<Unit> enemies = CardEffectExecutor.GetAllEnemies(caster);
        List<Unit> frontEnemies = new List<Unit>();
        List<Unit> backEnemies = new List<Unit>();
        foreach (Unit enemy in enemies)
        {
            if (enemy.IsFrontline())
            {
                frontEnemies.Add(enemy);
            }
            else
            {
                backEnemies.Add(enemy);
            }
        }

        if (caster.IsFrontline())
        {
            foreach(Unit enemy in frontEnemies)
            {
                enemy.TakeDamage(caster.ModifyOutgoingDamage(2), DamageType.Direct, caster);

                //SE LETAL
                if (enemy.currentHP <= 0)
                {
                    yield return new WaitForSeconds(0.15f);

                    foreach(Unit e in backEnemies)
                    {
                        e.TakeDamage(caster.ModifyOutgoingDamage(4), DamageType.Direct, caster);
                    }
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            foreach (Unit enemy in frontEnemies)
            {
                enemy.TakeDamage(caster.ModifyOutgoingDamage(4), DamageType.Direct, caster);
                TryPush(enemy);
            }
        }
        yield break;
    }
    private void TryPush(Unit target)
    {
        if (Random.Range(0, 100) < 40)
        {
            BoardManager.Instance.TryMoveUnit(target, 2, false);
        }
    }
}