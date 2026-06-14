using CardData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Outlaw/3in1")]
public class ThreeInOneEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        int bleedValue = 1;
        int critGain = 5;
        if (caster.IsFrontline())
        {
            for (int i = 0; i < 3; i++)
            {
                
                target.TakeDamage(caster.ModifyOutgoingDamage(3), DamageType.Direct, caster);
                target.AddStatus(new BleedEffect { value = bleedValue });
                bleedValue++;
                if (target.currentHP <= 0)
                {
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                target.TakeDamage(caster.ModifyOutgoingDamage(3), DamageType.Direct, caster);
                caster.AddStatus(new CritEffect {value = critGain});
                critGain += 5;
                if (target.currentHP <= 0)
                {
                    break;
                }
            }
        }
            yield break;
    }
}