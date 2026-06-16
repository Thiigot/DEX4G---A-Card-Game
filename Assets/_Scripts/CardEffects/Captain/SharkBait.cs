using CardData;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[CreateAssetMenu(menuName = "Captain/Shark Bait")]
public class SharkBaitEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {

        if (caster.IsFrontline())
        {
            target.AddStatus(new WeaknessEffect { value = 2 });
        }
        else
        {
            BoardManager.Instance.TryMoveUnit(caster, 3, true);
            yield return new WaitForSeconds(0.15f);
            target.AddStatus(new RetaliateEffect { value = 20 });
        }
        yield break;
    }
}