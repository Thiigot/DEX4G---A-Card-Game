using CardData;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Outlaw/In Sight")]
public class InSightEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster,Unit target,Card card)
    {
        if (caster.IsFrontline())
        {
            BoardManager.Instance.TryMoveUnit(
                caster,
                3,
                false
            );
        }

        caster.isChanneling = true;

        caster.channelResolveAction = () =>
        {
            if (target != null)
            {
                target.TakeDamage(999999, DamageType.Direct,caster,true,false);
            }
        };

        caster.AddStatus(
            new ChannelEffect()
            {
                value = 2
            }
        );

        TurnManager.Instance.EndPlayerTurn();

        yield break;
    }
}