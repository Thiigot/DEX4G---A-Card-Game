using CardData;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Outlaw/Trail Blood")]
public class TrailBloodEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        List<Unit> enemies = CardEffectExecutor.GetAllEnemies(caster);
        if (enemies.Count == 0) yield break;

        Unit randomEnemy = enemies[Random.Range(0, enemies.Count)];
        BleedEffect bleed = randomEnemy.GetStatus<BleedEffect>();
        int hpBefore = randomEnemy.currentHP;

        //RANDOM ENEMY ATTACK
        randomEnemy.TakeDamage(caster.ModifyOutgoingDamage(3), DamageType.Direct, caster);
        bool killed = hpBefore > 0 && randomEnemy.currentHP <= 0;
        if (!killed) yield break;


        if (caster.IsFrontline())
        {
            enemies.Remove(randomEnemy);
            if (enemies.Count <= 0)
                yield break;
            if (bleed.value > 0)
            {
                //TRANSFER BLEED
                Unit anotherEnemy = enemies[Random.Range(0, enemies.Count)];
                anotherEnemy.AddStatus(new BleedEffect { value = bleed.value });
            }
        }
        else
        {
            yield return RepeatAttackChain(caster);
        }

        yield break;
    }
    private IEnumerator RepeatAttackChain(Unit caster)
    {
        while (true)
        {
            List<Unit> enemies = CardEffectExecutor.GetAllEnemies(caster);

            if (enemies.Count == 0) yield break;

            Unit randomEnemy = enemies[Random.Range(0, enemies.Count)];

            int hpBefore = randomEnemy.currentHP;

            randomEnemy.TakeDamage(caster.ModifyOutgoingDamage(3),DamageType.Direct,caster);

            bool killed = hpBefore > 0 && randomEnemy.currentHP <= 0;

            yield return new WaitForSeconds(0.2f);

            if (!killed) yield break;
        }
    }
}