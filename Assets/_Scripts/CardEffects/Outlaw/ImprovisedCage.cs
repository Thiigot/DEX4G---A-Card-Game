using UnityEngine;
using CardData;
using System.Collections;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "Outlaw/Improvised Cage")]
public class ImprovisedCageEffect : CardSpecialEffect
{

    public override IEnumerator OnPlayCoroutine(
        Unit caster,
        Unit target,
        Card card)
    {
        if (target == null)
            yield break;

        target.AddStatus(
            new StunEffect()
            {
                value = 1
            }
        );

        yield return new WaitForSeconds(0.2f);

        if (caster.IsFrontline())
        {
            BleedEffect bleed = target.GetStatus<BleedEffect>();
            if (bleed != null)
            {
                TurnManager.Instance
                    .GrantExtraTurn(caster);

                Debug.Log(
                    $"{caster.unitName} ganhou turno extra!"
                );
            }
        }
        else
        {
            MarkEffect mark = target.GetStatus<MarkEffect>();
            if (mark != null)
            {
                TurnManager.Instance.GrantExtraTurn(caster);

                Debug.Log(
                    $"{caster.unitName} ganhou turno extra!"
                );
            }
        }

        yield break;
    }
}