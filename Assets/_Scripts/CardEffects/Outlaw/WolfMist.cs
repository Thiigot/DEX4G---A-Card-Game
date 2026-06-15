using CardData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Outlaw/Wolf in the Mist")]
public class WolfMistEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        if (caster.IsFrontline())
        {
            caster.AddStatus(new WolfBleedEffect{value = 1});
        }
        else
        {
            caster.AddStatus(new StealthEffect{value = 1});
            caster.AddStatus(new NextAttackCritEffect { value = 1 });
            caster.AddStatus(new NextAttackAdvanceEffect { value = 3 });

        }
        yield break;
    }
}