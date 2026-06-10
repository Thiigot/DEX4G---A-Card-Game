using UnityEngine;
using CardData;
using System.Collections;

[CreateAssetMenu(menuName = "Wanderer/Metallic Meditation")]
public class MetallicMeditationEffect : CardSpecialEffect
{
    public int critGain = 30;
    public int dodgeGain = 30;

    public override IEnumerator OnPlayCoroutine(
        Unit caster,
        Unit target,
        Card card)
    {
        if (caster.IsFrontline())
        {
            yield return FrontEffect(caster);
        }
        else
        {
            yield return BackEffect(caster);
        }
    }

    IEnumerator FrontEffect(Unit caster)
    {
        Card drawnCard = null;

        yield return caster.StartCoroutine(
            caster.DrawSingleCard(
                card => drawnCard = card
            )
        );

        if (drawnCard == null)
            yield break;

        yield return new WaitForSeconds(0.3f);

        //----------------------------------
        // CUSTO ÍMPAR
        //----------------------------------

        if (drawnCard.cardMana % 2 != 0)
        {
            Debug.Log($"crit antes: {caster.critChance} ");
            caster.AddStatus(
                new CritEffect()
                {
                    value = critGain
                }
            );
            Debug.Log($"crit depois: {caster.critChance} ");
        }

        //----------------------------------
        // CUSTO PAR
        //----------------------------------

        else
        {
            Debug.Log($"dodge antes: {caster.dodgeChance} ");
            caster.AddStatus(
                new DodgeEffect()
                {
                    value = dodgeGain
                }
            );

            Debug.Log($"dodge depois: {caster.dodgeChance} ");
        }
    }

    IEnumerator BackEffect(Unit caster)
    {
        caster.DrawCards(2);
        yield break;
    }
}