using CardData;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[CreateAssetMenu(menuName = "Captain/WarRum")]
public class WarRumEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {

        if (caster.IsFrontline())
        {
            caster.Heal(6);
            yield return new WaitForSeconds(0.15f);
            caster.AddStatus(new ProtectionEffect { value = 20 });
        }
        else
        {
            caster.Heal(2);
            yield return new WaitForSeconds(0.15f);
            BoardManager.Instance.TryMoveUnit(caster, 2, true);
        }
        yield break;
    }
}