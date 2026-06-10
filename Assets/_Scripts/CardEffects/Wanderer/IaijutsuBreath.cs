using UnityEngine;
using CardData;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Wanderer/Iaijutsu Breathing")]
public class IaijutsuBreathingEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(
        Unit caster,
        Unit target,
        Card card)
    {
        bool continueCombo = true;

        while (continueCombo)
        {
            continueCombo = false;

            yield return new WaitForSeconds(0.5f);

            Card topCard = caster.deckManager.DrawCard();

            if (topCard == null)
                yield break;

            //------------------------------------------------
            // MOSTRAR CARTA REVELADA
            //------------------------------------------------

            yield return ShowRevealedCard(topCard);

            yield return new WaitForSeconds(0.5f);

            //------------------------------------------------
            // ESCOLHER ALVO
            //------------------------------------------------

            Unit chosenTarget = FindTargetForCard(
                caster,
                topCard
            );

            //------------------------------------------------
            // USAR CARTA
            //------------------------------------------------

            Debug.Log(
                $"{caster.unitName} usou {topCard.cardName}via Iaijutsu Breathing!"
            );

            if (topCard.specialEffect != null)
            {
                yield return topCard.specialEffect.OnDraw(
                    caster,
                    topCard
                );
            }

            yield return CardEffectExecutor.ExecuteCardCoroutine(
                caster,
                chosenTarget,
                topCard
            );

            caster.deckManager.AddToDiscard(topCard);

            yield return new WaitForSeconds(0.5f);

            //------------------------------------------------
            // REPETE SE FOR ATAQUE
            //------------------------------------------------

            if (topCard.cardType == CardType.Attack)
            {
                continueCombo = true;
                Debug.Log("Combo continua!");
                yield return new WaitForSeconds(0.5f);
            }
        }
    }


    IEnumerator ShowRevealedCard(Card card)
    {
        Debug.Log($"Revelou: {card.cardName}");

        // depois você troca por UI real

        yield return new WaitForSeconds(0.5f);
    }

    Unit FindTargetForCard(
        Unit caster,
        Card card)
    {
        List<Unit> enemies = new();

        foreach (var unit in Object.FindObjectsOfType<Unit>())
        {
            if (unit.isPlayer != caster.isPlayer)
                enemies.Add(unit);
        }

        if (enemies.Count == 0)
            return null;

        return enemies[
            Random.Range(0, enemies.Count)
        ];
    }
}