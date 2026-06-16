using CardData;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[CreateAssetMenu(menuName = "Captain/Shot n Chop")]
public class ShotnChopEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {

        if (caster.IsFrontline())
        {
            //CUTLASS
            int damage = caster.ModifyOutgoingDamage(caster.attack * 2);
            target.TakeDamage(damage, DamageType.Direct, caster);
            BoardManager.Instance.TryMoveUnit(caster,3,false);
        }
        else
        {
            //FLINTLOCK
            List<Unit> targets = CardEffectExecutor.GetAllEnemies(caster);
            foreach(Unit enemy in targets)
            {
                enemy.TakeDamage(caster.ModifyOutgoingDamage(2),DamageType.Direct, caster);
                BoardManager.Instance.TryMoveUnit(caster, 3, true);
            }
        }
        card.shuffleIntoDeckInsteadOfDiscard = true;
        yield break;
    }
}