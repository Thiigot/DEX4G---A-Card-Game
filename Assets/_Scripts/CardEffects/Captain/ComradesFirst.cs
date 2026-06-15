using CardData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Captain/Comrades First")]
public class ComradesFirstEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        if (caster.IsFrontline())
        {
            //TAUNT
            caster.AddStatus(new TauntEffect { owner = target, taunter = caster, value = 1});
            //PROTECTION
            caster.AddStatus(new ProtectionEffect { value = 10 });
        }
        else
        {
            List<Unit> allies = CardEffectExecutor.GetAllAllies(caster);
            foreach (Unit a in allies)
            {
                a.AddStatus(new ProtectionEffect { value = 10 });
            }
        }
        yield break;
    }
}