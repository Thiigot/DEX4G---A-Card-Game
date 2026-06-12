using UnityEngine;
using CardData;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Outlaw/Desert Eclipse")]
public class DesertEclipseEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        if (caster.IsFrontline())
        {
            BoardManager.Instance.TryMoveUnit(caster,3,false);
        }
        yield return new WaitForSeconds(0.2f);
        caster.AddStatus(new StealthEffect()
            {
                value = 1
            }
        );
        yield return new WaitForSeconds(0.2f);
        caster.AddStatus(new NextAttackBonusEffect()
            {
                multiplier = 3f,
                value = 1
            }   
        );

        yield return new WaitForSeconds(0.2f);
        TurnManager.Instance.playerFinishedTurn = true;
        yield break;
    }
}

