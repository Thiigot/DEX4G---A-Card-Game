using CardData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Captain/GetHandsDirty")]
public class GetHandsDirtyEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        if (caster.IsFrontline())
        {
            for(int i =0; i< 2; i++)
            {
                //TAUNT
                caster.AddStatus(new TauntEffect { owner = target, taunter = caster, value = 2 });

                yield return new WaitForSeconds(0.15f);

                //PROTECTION
                caster.AddStatus(new ProtectionEffect { value = 10 });

                yield return new WaitForSeconds(0.25f);
            }
        }
        else
        {
            BoardManager.Instance.TryMoveUnit(caster, 3, true);
            yield return new WaitForSeconds(0.15f);
            caster.AddStatus(new RetaliateEffect { value = 30 });
        }
        yield break;
    }
}