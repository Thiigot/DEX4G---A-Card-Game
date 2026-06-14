using CardData;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Outlaw/RopePowder")]
public class RopePowderEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    { 
        if(caster.IsFrontline())
        {
            //ATTACK
            target.TakeDamage(caster.ModifyOutgoingDamage(3), DamageType.Direct, caster);
            if (target.IsFrontline())
            {
                yield return new WaitForSeconds(0.2f);
                //RETREAT 2
                BoardManager.Instance.TryMoveUnit(caster, 2, false);
            }
        }
        else
        {
            BoardManager.Instance.TryMoveUnit(target, 2, true);
        }
        yield break;    
    }
}