using UnityEngine;
using CardData;
using System.Collections;

[CreateAssetMenu(menuName = "Wanderer/Iaijutsu Strike")]
public class IaijutsuStrikeEffect : CardSpecialEffect
{
    public int damage = 4;

    public override IEnumerator OnPlayCoroutine(
        Unit caster,
        Unit target,
        Card card)
    {
        if (target == null)
            yield break;

        //------------------------------------------------
        // FRONT
        //------------------------------------------------
        if (caster.IsFrontline())
        {
            bool crit = caster.TryCrit();

            int finalDamage = caster.ModifyOutgoingDamage(damage,false);

            if (crit)
                finalDamage *= 2;

            target.TakeDamage(
                finalDamage,
                DamageType.Direct,
                caster
            );

            int zenAmount = crit ? 2 : 1;

            caster.deckManager.ZenBladeGen(
                zenAmount,
                caster
            );

            Debug.Log(
                $"{caster.unitName} usou Iaijutsu Strike. Crit={crit}. Criou {zenAmount} Zen Blade."
            );
        }

        //------------------------------------------------
        // BACK
        //------------------------------------------------
        else
        {
            //float originalCrit = caster.critChance;

            //caster.critChance = 0;

            target.TakeDamage(
                caster.ModifyOutgoingDamage(damage, false),
                DamageType.Direct,
                caster
            );

            //caster.critChance = originalCrit;

            caster.deckManager.ZenBladeGen(
                2,
                caster
            );

            caster.currentMana += 1;

            Debug.Log(
                $"{caster.unitName} usou Iaijutsu Strike (Back). +2 Zen Blade e +1 Mana."
            );
        }

        yield return new WaitForSeconds(0.1f);
    }
}