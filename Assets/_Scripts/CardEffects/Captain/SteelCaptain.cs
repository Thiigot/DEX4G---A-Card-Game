using CardData;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[CreateAssetMenu(menuName = "Captain/Steel Captain")]
public class SteelCaptainEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {

        if (caster.IsFrontline())
        {
            caster.AddStatus(new CritImmunityEffect { value = 2 });
            caster.AddStatus(new WeaknessEffect { value = 30 });

        }
        else
        {
            BoardManager.Instance.TryMoveUnit(caster, 1, true);
            yield return new WaitForSeconds(0.2f);
            target.AddStatus(new TauntEffect { taunter = caster, value = 1 });
            caster.AddStatus(new MarkEffect { value = 1 });
        }
        yield break;
    }
}