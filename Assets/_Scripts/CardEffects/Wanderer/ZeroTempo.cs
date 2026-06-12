using UnityEngine;
using CardData;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Wanderer/Zero Tempo")]
public class ZeroTempoEffect : CardSpecialEffect
{
    public int energyGain = 3;

    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        bool isFastest = IsFastestUnit(caster);

        if (!isFastest)
        {
            Debug.Log("Zero Tempo falhou.");
            yield break;
        }

        if (caster.IsFrontline())
        {
            caster.AddStatus(
                new NextAttackBonusEffect()
                {
                    multiplier = 1.5f,
                    value = 1
                }
            );

            Debug.Log(
                $"Próximo ataque causa +{1.5}x dano."
            );
        }
        else
        {
            caster.currentMana += energyGain;
            caster.handManager.manaManager.UpdateUI();
        }

        yield break;
    }

    private bool IsFastestUnit(Unit caster)
    {
        Unit[] units =
            FindObjectsByType<Unit>(
                FindObjectsSortMode.None
            );

        foreach (Unit unit in units)
        {
            if (unit == null)
                continue;

            if (unit == caster)
                continue;

            if (unit.speed > caster.speed)
                return false;
        }

        return true;
    }
}