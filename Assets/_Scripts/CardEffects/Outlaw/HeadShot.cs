using UnityEngine;
using CardData;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "Outlaw/Head Shot")]
public class HeadShotEffect : CardSpecialEffect
{
    public int markAmount = 1;

    public override IEnumerator OnPlayCoroutine(Unit caster,Unit target,Card card)
    {
        if (target == null)
            yield break;

        if (caster.IsFrontline())
        {
            target.AddStatus(
                new MarkEffect()
                {
                    value = markAmount
                }
            );

            target.AddStatus(
                new MarkEffect()
                {
                    value = markAmount
                }
            );
        }
        else
        {
            target.AddStatus(
                new MarkEffect()
                {
                    value = markAmount
                }
            );

            List<Unit> enemies =
                CardEffectExecutor.GetAllEnemies(caster);

            enemies.Remove(target);

            if (enemies.Count > 0)
            {
                Unit secondTarget = enemies[Random.Range(0,enemies.Count)];

                secondTarget.AddStatus(
                    new MarkEffect()
                    {
                        value = markAmount
                    }
                );
            }
        }

        yield break;
    }
}