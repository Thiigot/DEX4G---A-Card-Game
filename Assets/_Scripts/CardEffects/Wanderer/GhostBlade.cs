using UnityEngine;
using CardData;
using System.Collections;

[CreateAssetMenu(menuName = "Wanderer/Ghost Blade")]
public class GhostBladeEffect : CardSpecialEffect
{
    [Header("Front")]
    public int duration = 1;

    [Header("Back")]
    public int chargeAmount = 2;
    public int energyGain = 2;

    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        if (caster.IsFrontline())
        {
            FrontEffect(caster);
        }
        else
        {
            BackEffect(caster);
        }
        yield break;
    }

    void FrontEffect(Unit caster)
    {
        caster.AddStatus(new IgnoreProtectionEffect
        {
            value = duration
        });

        Debug.Log(
            $"{caster.unitName} agora ignora proteção até o final do turno."
        );
    }

    void BackEffect(Unit caster)
    {
        caster.currentMana += energyGain;
        caster.handManager.manaManager.UpdateUI();

        BoardManager.Instance.TryMoveUnit(
            caster,
            chargeAmount,
            true
        );

        Debug.Log(
            $"{caster.unitName} avançou {chargeAmount} e ganhou +{energyGain} energia."
        );
    }
}