using UnityEngine;
using CardData;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Wanderer/Last Draw")]
public class LastDrawEffect : CardSpecialEffect
{
    public int damage = 4;

    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {

        List<Card> cardsToDiscard = new List<Card>(caster.hand);

        while(cardsToDiscard.Count > 0)
        {
            Card discardedCard = cardsToDiscard[0];
            cardsToDiscard.RemoveAt(0);

            // Descarta carta
            caster.hand.Remove(discardedCard);
            caster.deckManager.AddToDiscard(discardedCard);
            yield return caster.StartCoroutine(caster.handManager.DiscardCardVisual(discardedCard,caster.deckManager));

            yield return new WaitForSeconds(0.1f);

            // Ataca inimigo aleatório
            List<Unit> enemies = CardEffectExecutor.GetAllEnemies(caster);
            if(enemies.Count > 0)
            {
                Unit randomEnemy = enemies[Random.Range(0, enemies.Count)];
                randomEnemy.TakeDamage(
                caster.ModifyOutgoingDamage(damage),
                DamageType.Direct,
                caster
                );
                Debug.Log(
                    $"{caster.unitName} atacou {randomEnemy.unitName}"
                );
            }
            yield return new WaitForSeconds(0.2f);
        }
        // Finaliza turno
        TurnManager.Instance.playerFinishedTurn = true;

        Debug.Log("Last Draw encerrou o turno.");
    }
}