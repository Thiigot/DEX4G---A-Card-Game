using CardData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Captain/All Sails set!")]
public class AllSailsSetEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        if (caster.IsFrontline())
        {
            if(caster.CurrentSlot == BoardManager.Instance.GetFrontMostSlot(caster.isPlayer))
            {
                caster.currentMana += 3;
                caster.handManager.manaManager.UpdateUI();
            }
        }
        else
        {
            BoardManager.Instance.TryMoveUnit(caster, 3, true);
            caster.speed *= 2;
        }
            yield break;
    }
}