using CardData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Captain/Captain's Orders")]
public class CaptainOrdersEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        List<Unit> allies = CardEffectExecutor.GetAllAllies(caster);
        if (caster.IsFrontline())
        {
            foreach(Unit a in allies)
            {
                a.AddStatus(new ProtectionEffect { value = 10 });
            }
        }
        else
        {
            foreach (Unit a in allies)
            {
                a.AddStatus(new DamageModifierEffect { value = 20 });
            }
        }
        yield break;
    }
}