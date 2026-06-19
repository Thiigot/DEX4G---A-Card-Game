using CardData;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[CreateAssetMenu(menuName = "Captain/TaketoTheBottom")]
public class TaketoTheBottomEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {

        if (caster.IsFrontline())
        {
            caster.AddStatus(new BottomEffect { value = 5 });
        }
        else
        {
            caster.AddStatus(new BottomBackEffect { value = 2 });
        }
        yield break;
    }
}