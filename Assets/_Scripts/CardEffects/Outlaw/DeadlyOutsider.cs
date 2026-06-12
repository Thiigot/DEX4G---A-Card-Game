using CardData;
using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(menuName = "Outlaw/Deadly Outsider")]
public class DeadlyOutsiderEffect : CardSpecialEffect
{
    public int critGain = 20;
    public int markAmount = 2;
    public int damage = 4;

    public override IEnumerator OnPlayCoroutine(Unit caster,Unit target,Card card)
    {
        if (target == null)
            yield break;

        if (caster.IsFrontline())
        {
            yield return FrontEffect(
                caster,
                target
            );
        }
        else
        {
            yield return BackEffect(
               caster,
               target
           );
        }
    }
    private IEnumerator FrontEffect(Unit caster, Unit target)
    {
        BleedEffect bleed = target.GetStatus<BleedEffect>();

        if (bleed == null || bleed.value < 5)
        {
            Debug.Log("Bleed insuficiente.");
            yield break;
        }
        //EXECUTE
        target.TakeDamage(
            999999,
            DamageType.Direct,
            caster,
            true
        );
        yield return new WaitForSeconds(0.2f);
        //MANA
        caster.currentMana++;
        caster.handManager.manaManager.UpdateUI();
        yield return new WaitForSeconds(0.2f);
        //CRIT
        caster.AddStatus(
            new CritEffect()
            {
                value = critGain
            }
        );
        yield break;
    }

    private IEnumerator BackEffect(Unit caster, Unit target)
    {
        target.AddStatus(
            new MarkEffect()
            {
                value = markAmount
            }
        );

        for (int i = 0; i < 3; i++)
        {
            if (target == null)
                break;

            if (target.currentHP <= 0)
                break;

            target.TakeDamage(caster.ModifyOutgoingDamage(damage), DamageType.Direct, caster);
            if (target.currentHP <= 0)
            {
                Debug.Log("LETHAL!");
                yield return new WaitForSeconds(0.2f);
                //ENERGY 1
                caster.currentMana++;
                caster.handManager.manaManager.UpdateUI();
                //DRAW 1
                caster.DrawCards(1);
            }

            yield return new WaitForSeconds(0.15f);
        }
    }
}