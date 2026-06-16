using CardData;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[CreateAssetMenu(menuName = "Captain/Steel Storm")]
public class SteelStormEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {

        if (caster.IsFrontline())
        {
            List<Unit> enemies = CardEffectExecutor.GetAllEnemies(caster);
            foreach (Unit enemy in enemies)
            {
                enemy.TakeDamage(caster.ModifyOutgoingDamage(2), DamageType.Direct, caster);
            }
        }
        else
        {
            target.TakeDamage(caster.ModifyOutgoingDamage(2), DamageType.Direct, caster);
            yield return new WaitForSeconds(0.2f);
            BoardManager.Instance.TryMoveUnit(caster,3, true);
            caster.AddStatus(new ProtectionEffect { value = 10 });
        }
        yield break;
    }
}
