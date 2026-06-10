using UnityEngine;
using CardData;
using System.Collections;

[CreateAssetMenu(menuName = "Wanderer/Infinity Sheath")]
public class InfinitySheathEffect : CardSpecialEffect
{
    public int damage = 4;

    public override IEnumerator OnPlayCoroutine(
        Unit caster,
        Unit target,
        Card card)
    {
        //--------------------------------
        // X = mana atual
        //--------------------------------

        int repeatCount = caster.currentMana;

        if (repeatCount <= 0)
            yield break;

        //--------------------------------
        // Gasta toda mana
        //--------------------------------

        caster.currentMana = 0;

        FindAnyObjectByType<ManaManagerSTS>().UpdateUI();
        Debug.Log(
            $"{caster.unitName} usou Infinity Sheath ({repeatCount} repetições)"
        );

        //--------------------------------
        // Repete X vezes
        //--------------------------------

        for (int i = 0; i < repeatCount; i++)
        {

            Unit randomTarget =
                CardEffectExecutor.GetAutomaticTarget(
                    caster,
                    card
                );

            if (randomTarget != null)
            {
                randomTarget.TakeDamage(
                    caster.ModifyOutgoingDamage(damage),
                    DamageType.Direct,
                    caster
                );
            }
            yield return new WaitForSeconds(0.2f);

            caster.deckManager.ZenBladeGen(
                1,
                caster
            );

            yield return new WaitForSeconds(0.4f);
        }
    }
}