using UnityEngine;
using CardData;
using System.Collections;

[CreateAssetMenu(menuName = "Outlaw/Dust Track")]
public class DustTrackEffect : CardSpecialEffect
{
    public int critGain = 20;
    public int speedGain = 5;

    public override IEnumerator OnPlayCoroutine(Unit caster,Unit target,Card card)
    {
        if (caster.IsFrontline())
        {
            // Recua 1
            BoardManager.Instance.TryMoveUnit(caster, 1,false);

            yield return new WaitForSeconds(0.15f);

            BleedEffect bleed = target.GetStatus<BleedEffect>();

            if (target != null && bleed != null)
            {
                caster.AddStatus(
                    new StealthEffect()
                    {
                        value = 1
                    }
                );

                caster.AddStatus(
                    new CritEffect()
                    {
                        value = critGain
                    }
                );
            }
        }
        else
        {
            // Avança 1
            BoardManager.Instance.TryMoveUnit(caster,1,true);

            yield return new WaitForSeconds(0.15f);

            MarkEffect mark = target.GetStatus<MarkEffect>();

            if (target != null && mark != null)
            {
                yield return caster.StartCoroutine(caster.DrawCardsAnimatedPublic(1));

                caster.speed += speedGain;
            }
        }

        yield break;
    }
}