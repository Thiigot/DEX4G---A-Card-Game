using CardData;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Outlaw/Red Tango")]
public class RedTangoEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster,Unit target,Card card)
    {

        caster.AddStatus(new RedTangoStatus()
            {
                value = 1,
                bleedRetaliate = caster.IsFrontline(),
                critRetaliate = caster.IsBackline()
            }
        );
        yield break;
    }
}