using CardData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Captain/Cut Throat")]
public class CutThroatEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        target.TakeDamage(caster.ModifyOutgoingDamage(2), DamageType.Direct, caster);
        if (caster.IsFrontline())
        {
            if (caster.IsTauntingSomeone())
            {
                caster.AddStatus(new CritEffect { value = 70 });
            }
        }
        else
        {
            caster.AddStatus(new CritEffect { value = 20 });
            yield return new WaitForSeconds(0.15f);
            BoardManager.Instance.TryMoveUnit(caster, 2, true);
        }
        yield break;
    }
}