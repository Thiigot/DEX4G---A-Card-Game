using CardData;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[CreateAssetMenu(menuName = "Captain/Salty Blade")]
public class SaltyBladeEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {

        if (caster.IsFrontline())
        {
            if (caster.IsTauntingSomeone())
            {
                caster.AddStatus(new CritEffect { value = 100 });
            }
        }
        else
        {
            caster.AddStatus(new CritEffect { value = 30 });
            BoardManager.Instance.TryMoveUnit(caster, 2, true);
        }
        yield break;
    }
}