using CardData;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[CreateAssetMenu(menuName = "Captain/Rum Heals Everything")]
public class RumHealsEverythingEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        List<Unit> allies = CardEffectExecutor.GetAllAllies(caster);

        if (caster.IsFrontline())
        {
            foreach(Unit ally in allies)
            {
                ally.Heal(4);
                TryWeakness(ally);
            }
        }
        else
        {
            foreach (Unit ally in allies)
            {
                if (TryFail())
                {
                    ally.Heal(4);
                }
            }
        }
        yield break;
    }

    private void TryWeakness(Unit target)
    {
        if (Random.Range(0, 100) < 10)
        {
            target.AddStatus(new WeaknessEffect { value = 1 });
        }
    }
    private bool TryFail()
    {
        if (Random.Range(0, 100) < 40)
        {
            return false;
        }
        return true;
    }
}