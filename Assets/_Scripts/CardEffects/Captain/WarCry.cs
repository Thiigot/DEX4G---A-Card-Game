using CardData;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[CreateAssetMenu(menuName = "Captain/WarCry")]
public class WarCryEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {

        if (caster.IsFrontline())
        {
            List<Unit> enemies = CardEffectExecutor.GetAllEnemies(caster);
            foreach(Unit enemy in enemies)
            {
                enemy.AddStatus(new WeaknessEffect { value = 20 });
            }
        }
        else
        {
            List<Unit> allies = CardEffectExecutor.GetAllAllies(caster);
            foreach (Unit ally in allies)
            {
                ally.AddStatus(new ProtectionEffect { value = 20 });
            }
        }
        yield break;
    }
}