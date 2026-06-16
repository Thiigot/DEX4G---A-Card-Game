using CardData;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[CreateAssetMenu(menuName = "Captain/Seven Harbour Queen")]
public class SevenHarbourQueenEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {

        if (caster.IsFrontline())
        {
            caster.AddStatus(new DebuffImmunityEffect { value =2  });
            caster.currentMana++;
            caster.handManager.manaManager.UpdateUI();
        }
        else
        {
            BoardManager.Instance.TryMoveUnit(caster,3,true);
            int prot = caster.GetStatus<ProtectionEffect>().value;
            caster.AddStatus(new ProtectionEffect { value = prot });
        }
        yield break;
    }
}