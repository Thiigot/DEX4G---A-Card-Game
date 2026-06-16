using CardData;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[CreateAssetMenu(menuName = "Captain/Walk the Plank!")]
public class WalkPlankEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {

        if (caster.IsFrontline())
        {
            target.TakeDamage(caster.ModifyOutgoingDamage(2), DamageType.Direct, caster);
            yield return new WaitForSeconds(0.2f);
            BoardManager.Instance.TryMoveUnit(target, 3, false);
            TryApplyStun(target);

        }
        else
        {
            target.TakeDamage(caster.ModifyOutgoingDamage(2), DamageType.Direct, caster);
            BoardManager.Instance.TryMoveUnit(caster, 3, true);
            yield return new WaitForSeconds(0.2f);
            caster.AddStatus(new RetaliateEffect { value = 30 });
        }
        yield break;
    }

    private void TryApplyStun(Unit target)
    {
        if (Random.Range(0, 100) < 30)
        {
            target.AddStatus(new StunEffect { value = 1 });
        }
    }
}