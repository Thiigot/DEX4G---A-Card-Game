using UnityEngine;
using CardData;
using System.Collections;

[CreateAssetMenu(menuName = "Wanderer/Iaido")]
public class IaidoEffect : CardSpecialEffect
{
    public int damage = 4;

    public int movementAmount = 2;

    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        // Ataque
        target.TakeDamage(
            caster.ModifyOutgoingDamage(damage),
            DamageType.Direct,
            caster
        );


        // Cria Zen Blades
        int zenToCreate = caster.cardsDrawnThisTurn;
        caster.deckManager.ZenBladeGen(zenToCreate, caster);

        // Movimento
        if (caster.IsFrontline())
        {
            // Retreat 2
            BoardManager.Instance.TryMoveUnit(
                caster,
                movementAmount,
                false
            );
        }
        else
        {
            // Charge 2
            BoardManager.Instance.TryMoveUnit(
                caster,
                movementAmount,
                true
            );
        }

        Debug.Log($"{caster.unitName} criou {zenToCreate} Zen Blades.");
        yield break;
    }
}