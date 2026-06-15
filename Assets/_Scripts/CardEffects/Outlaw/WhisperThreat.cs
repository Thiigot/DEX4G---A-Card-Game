using CardData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Outlaw/Whispered Threat")]
public class WhisperThreatEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        List<Unit> enemies = CardEffectExecutor.GetAllEnemies(caster);
        List<Unit> markedEnemies = new List<Unit>();
        List<Unit> bleedingEnemies = new List<Unit>();
        foreach (Unit enemy in enemies)
        {
            if (enemy.GetStatus<BleedEffect>() != null)
            {
                bleedingEnemies.Add(enemy);
            }
            if (enemy.GetStatus<MarkEffect>() != null)
            {
                markedEnemies.Add(enemy);
            }
        }
        if (caster.IsFrontline())
        {
            foreach (Unit enemy in bleedingEnemies)
            {
                caster.AddStatus(new CritEffect{ value = 15 });
            }
        }
        else
        {
            foreach (Unit enemy in markedEnemies)
            {
                enemy.AddStatus(new BleedEffect { value = 3 });
            }
        }
        yield break;
    }
}