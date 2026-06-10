using CardData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wanderer/Reflective Dash")]
public class ReflectiveDashEffect : CardSpecialEffect
{
    public int damage = 4;

    public override IEnumerator OnPlayCoroutine( Unit caster, Unit target, Card card)
    {
        //FRONTLINE EFFECT
        if (caster.IsFrontline())
        {
            //RETREAT 2
            BoardManager.Instance.TryMoveUnit(
                caster,
                2,
                false
            );
            //ATTACK
            target.TakeDamage(
            caster.ModifyOutgoingDamage(damage),
            DamageType.Direct,
            caster
            );
            if(target.currentHP <= 0)
            {
                Debug.Log("LETHAL!");
                yield return new WaitForSeconds(0.5f);
                //DRAW 1
                caster.DrawCards(1);
                //CHARGE 2
                BoardManager.Instance.TryMoveUnit(
                caster,
                2,
                true
                );
            }
        }
        //BACKLINE EFFECT
        else
        {
            //CHARGE 2
            
            BoardManager.Instance.TryMoveUnit(
                caster,
                2,
                true
            );
            //ATTACK
            target.TakeDamage(
            caster.ModifyOutgoingDamage(damage),
            DamageType.Direct,
            caster
            );
            if (target.currentHP <= 0)
            {
                Debug.Log("LETHAL!");
                yield return new WaitForSeconds(0.5f);
                //DRAW 1
                caster.DrawCards(1);
                //RETREAT 2
                BoardManager.Instance.TryMoveUnit(
                    caster,
                    2,
                    false
                );
            }
        }
        yield break;
    }
}