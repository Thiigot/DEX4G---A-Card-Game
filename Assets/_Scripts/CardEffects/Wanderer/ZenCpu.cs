using CardData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wanderer/Zen CPU")]
public class ZenCPUEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        if (caster.IsFrontline())
        {
            //DRAW 2
            yield return caster.StartCoroutine(
                caster.DrawCardsAnimatedPublic(2)
            );
        }
        else
        {
            //DRAW 4
            yield return caster.StartCoroutine(
                caster.DrawCardsAnimatedPublic(4)
            );

            yield return new WaitForSeconds(0.2f);

            //DISCARD 2 (RANDOM)
            #region DISCARD
            int discardAmount = Mathf.Min(2, caster.hand.Count);

            //for (int i = 0; i < discardAmount; i++)
            //{
            //    int randomIndex =
            //        Random.Range(0, caster.hand.Count);

            //    Card discardedCard =
            //        caster.hand[randomIndex];

            //    caster.hand.RemoveAt(randomIndex);

            //    caster.deckManager.AddToDiscard(
            //        discardedCard
            //    );

            //    yield return caster.StartCoroutine(
            //        caster.handManager.DiscardCardVisual(
            //            discardedCard,
            //            caster.deckManager
            //        )
            //    );

            //    yield return new WaitForSeconds(0.1f);
            //}
            #endregion

            List<Card> possibleDiscards = new List<Card>(caster.hand);
            for (int i = 0; i < discardAmount; i++)
            {
                int randomIndex =
                    Random.Range(0, possibleDiscards.Count);

                Card discardedCard =
                    possibleDiscards[randomIndex];

                possibleDiscards.RemoveAt(randomIndex);

                caster.hand.Remove(discardedCard);

                caster.deckManager.AddToDiscard(
                    discardedCard
                );

                yield return caster.StartCoroutine(
                    caster.handManager.DiscardCardVisual(
                        discardedCard,
                        caster.deckManager
                    )
                );
            }
        }

        yield break;
    }
}