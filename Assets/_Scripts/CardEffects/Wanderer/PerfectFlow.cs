using UnityEngine;
using CardData;
using System.Collections;

[CreateAssetMenu(menuName = "Wanderer/Perfect Flow")]
public class PerfectFlowEffect : CardSpecialEffect
{
    public int energyGain = 2;
    public int drawAmount = 2;

    public override IEnumerator OnPlayCoroutine(
        Unit caster,
        Unit target,
        Card card)
    {
        if (!caster.dodgedSinceLastTurn)
        {
            Debug.Log("Perfect Flow falhou: não houve dodge no turno passado.");
            yield break;
        }

        if (caster.IsFrontline())
        {
            //--------------------------------
            // FRONT
            //--------------------------------

            caster.currentMana += energyGain;

            ManaManagerSTS mana =
                Object.FindAnyObjectByType<ManaManagerSTS>();

            if (mana != null)
                mana.UpdateUI();

            Debug.Log(
                $"{caster.unitName} ganhou +{energyGain} Energy."
            );
        }
        else
        {
            //--------------------------------
            // BACK
            //--------------------------------

            caster.DrawCards(drawAmount);

            Debug.Log(
                $"{caster.unitName} comprou {drawAmount} cartas."
            );
        }

        yield break;
    }
}