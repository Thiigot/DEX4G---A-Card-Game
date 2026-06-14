using CardData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Outlaw/3B1S")]
public class ThreeBOneSEffect : CardSpecialEffect
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
            //3 ATTACKS
            for (int i = 0; i < 3; i++)
            {
                Unit randomfrontEnemy = frontEnemies[Random.Range(0, frontEnemies.Count)];
                Unit randomBackEnemy = backEnemies[Random.Range(0, backEnemies.Count)];

                randomfrontEnemy.TakeDamage(caster.ModifyOutgoingDamage(3),DamageType.Direct,caster);

                //SE LETAL
                if (randomfrontEnemy.currentHP <= 0)
                {
                    yield return new WaitForSeconds(0.15f);
                    BleedEffect bleed = randomfrontEnemy.GetStatus<BleedEffect>();
                    randomBackEnemy.AddStatus(new BleedEffect { value = bleed.value });
                }
                yield return new WaitForSeconds(0.2f);
            }
        }
        else
        {
            //3 ATTACKS
            for (int i = 0; i < 3; i++)
            {
                Unit randomBackEnemy = backEnemies[Random.Range(0, backEnemies.Count)];
                randomBackEnemy.TakeDamage(caster.ModifyOutgoingDamage(3), DamageType.Direct, caster, true);
            }
        }

        yield break;
    }
}