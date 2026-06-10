using CardData;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Wanderer/Timed Sheath")]
public class TimedSheathEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {

        //Card topCard = caster.deckManager.PeekTopCard();
        //if(topCard == null)
        //{
        //    caster.deckManager.Reshuffle();
        //    topCard = caster.deckManager.PeekTopCard();
        //}
        //yield return caster.StartCoroutine(
        //    caster.DrawCardsAnimatedPublic(1)
        //);
        Card topCard = null;
        yield return caster.StartCoroutine(caster.DrawSingleCard( c => topCard = c ));

        if (topCard == null)
            yield break;

        if (caster.IsFrontline())
        {
            if(topCard.cardType == CardType.Stance)
            {
                topCard.SetTemporaryCost(0);
                caster.handManager.RefreshCardVisual(topCard);
            }
        }
        else
        {
            if (topCard.cardType == CardType.Attack)
            {
                topCard.SetTemporaryCost(0);
                caster.handManager.RefreshCardVisual(topCard);
            }
        }
        yield return new WaitForSeconds(0.2f);
        yield break;
    }
}

