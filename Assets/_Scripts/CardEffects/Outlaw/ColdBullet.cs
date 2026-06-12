using UnityEngine;
using CardData;
using System.Collections;

[CreateAssetMenu(menuName = "Outlaw/Cold Bullet")]
public class ColdBulletEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        if (target == null)
            yield break;

        if (caster.IsFrontline())
        {
            BleedEffect bleed = target.GetStatus<BleedEffect>();

            if (bleed != null)
            {
                bleed.value *= 2;
            }
            target.UpdateStatusUI();
        }
        else
        {
            MarkEffect mark = target.GetStatus<MarkEffect>();

            if (mark != null)
            {
                mark.value *= 2;
            }
            target.UpdateStatusUI();
        }

        yield break;
    }
}