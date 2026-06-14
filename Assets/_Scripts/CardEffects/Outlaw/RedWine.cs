using CardData;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Outlaw/Red Wine Killshot")]
public class RedWineKillshotEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster,Unit target,Card card)
    {
        if (target == null)
            yield break;

        BleedEffect bleed = target.GetStatus<BleedEffect>();

        if (bleed == null)
            yield break;

        int stacks = bleed.value;

        int totalDamage = stacks * (stacks + 1) / 2;

        target.TakeDamage(totalDamage, DamageType.Direct, caster, true, false);

        target.RemoveStatus<BleedEffect>();

        yield break;
    }
}