using CardData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Outlaw/Powder Addiction")]
public class PowderAddictionEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        if (caster.IsFrontline())
        {
            //DAMAGE BONUS
            caster.AddStatus(new DamageModifierEffect {value = 50});

            yield return new WaitForSeconds(0.15f);

            //Bleed
            caster.AddStatus(new BleedEffect { value = 3});
        }
        else
        {
            //CRIT CHANCE
            caster.AddStatus(new CritEffect { value = 30});

            yield return new WaitForSeconds(0.15f);

            //DISCARD 2 (RANDOM)

            int discardAmount = Mathf.Min(2, caster.hand.Count);

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
