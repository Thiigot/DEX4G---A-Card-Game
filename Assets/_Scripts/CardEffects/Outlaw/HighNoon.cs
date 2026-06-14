using UnityEngine;
using CardData;
using System.Collections;

[CreateAssetMenu(menuName = "Outlaw/High Noon")]
public class HighNoonEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(
        Unit caster,
        Unit target,
        Card card)
    {
        if (target == null)
            yield break;

        //----------------------------------
        // FRONT
        //----------------------------------
        if (caster.IsFrontline())
        {
            int casterBleed = caster.GetBleedStacks();

            int targetBleed = target.GetBleedStacks();

            if (casterBleed < targetBleed)
            {
                target.TakeDamage(999999, DamageType.Direct, caster, true);
            }
            else if (targetBleed < casterBleed)
            {
                caster.TakeDamage(999999, DamageType.Direct, target, true);
            }
            else
            {
                Debug.Log("Empate no duelo.");
            }
        }

        //----------------------------------
        // BACK
        //----------------------------------
        else
        {
            float casterCrit = caster.critChance;

            float targetCrit = target.critChance;

            if (casterCrit > targetCrit)
            {
                target.TakeDamage(999999, DamageType.Direct, caster, true);

            }
            else if (targetCrit > casterCrit)
            {
                caster.TakeDamage(999999, DamageType.Direct, target, true);
            }
            else
            {
                Debug.Log("Empate no duelo.");
            }
        }

        yield break;
    }
}