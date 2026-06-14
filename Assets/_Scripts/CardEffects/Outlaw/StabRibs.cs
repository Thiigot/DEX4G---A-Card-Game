using CardData;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Outlaw/Stab Ribs")]
public class StabRibsEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        if (caster.IsFrontline())
        {
            //HEAVY BLEED
            target.AddStatus(new HeavyBleedEffect { value = 3 });
        }
        else
        {
            //CHARGE 2
            BoardManager.Instance.TryMoveUnit(caster, 2, true);

            target.AddStatus(new BleedEffect { value = 3 });
        }
            yield break;
    }
}