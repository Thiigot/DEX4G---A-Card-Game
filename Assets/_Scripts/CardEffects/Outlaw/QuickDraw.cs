using CardData;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Outlaw/QuickDraw")]
public class QuickDrawEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        if (caster.IsFrontline())
        {
            //DRAW 2
            yield return caster.StartCoroutine(
                caster.DrawCardsAnimatedPublic(4)
            );

            yield return new WaitForSeconds(0.2f);
            //RETREAT 2
            BoardManager.Instance.TryMoveUnit(caster,2,false);
        }
        else
        {
            BoardManager.Instance.TryMoveUnit(caster, 2, true);
            target.AddStatus(new WeaknessEffect {value= 10});
        }
        yield break;
    }
}