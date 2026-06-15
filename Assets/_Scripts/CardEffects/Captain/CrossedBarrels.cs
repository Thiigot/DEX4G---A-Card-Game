using CardData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Captain/Crossed Barrels")]
public class CrossedBarrelsEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        target.TakeDamage(caster.ModifyOutgoingDamage(2), DamageType.Direct, caster);

        if (caster.IsFrontline())
        {
            caster.AddStatus(new GuaranteedRetaliateEffect { value = 1});
        }
        else
        {
            caster.AddStatus(new RetaliateEffect { value = 30});
            yield return new WaitForSeconds(0.15f);
            BoardManager.Instance.TryMoveUnit(caster, 2, true);
        }
        yield break;
    }
}