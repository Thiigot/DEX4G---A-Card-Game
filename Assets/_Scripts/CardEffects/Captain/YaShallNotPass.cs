using CardData;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[CreateAssetMenu(menuName = "Captain/Ya Shall Not Pass!")]
public class ShallNotPassEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {

        if (caster.IsFrontline())
        {
            target.AddStatus(new TauntEffect{ taunter = caster, value = 2 });
        }
        else
        {
            BoardManager.Instance.TryMoveUnit(caster, 3, true);
            //BLOQUEIA ATAQUE INIMIGO
        }
        yield break;
    }
}