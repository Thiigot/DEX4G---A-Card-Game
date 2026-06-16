using CardData;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[CreateAssetMenu(menuName = "Captain/Victory Song")]
public class VictorySongEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {

        if (caster.IsFrontline())
        {
            List<Unit> allies = CardEffectExecutor.GetAllAllies(caster);
            foreach(Unit a in allies)
            {
                a.AddStatus(new RetaliateEffect { value = 20 });
            }
        }
        else
        {
            caster.AddStatus(new RetaliateEffect { value = 20 });
        }
        yield break;
    }
}