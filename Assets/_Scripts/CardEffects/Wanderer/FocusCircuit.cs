using UnityEngine;
using CardData;
using System.Collections.Generic;
using System.Collections;

[CreateAssetMenu(menuName = "Wanderer/Focus Circuit")]
public class FocusCircuitEffect : CardSpecialEffect
{
    [Header("Front")]
    public int speedGain = 3;
    public int retreatAmount = 2;

    [Header("Back")]
    public int dodgeGain = 15;
    public int advanceAmount = 2;

    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        if (caster.IsFrontline())
        {
            FrontEffect(caster);
        }
        else
        {
            BackEffect(caster);
        }
        yield break;
    }

    void FrontEffect(Unit caster)
    {
        // Ganha velocidade
        caster.speed += speedGain;
        
        // Recua 2 posições
        BoardManager.Instance.TryMoveUnit(
            caster,
            retreatAmount,
            false
        );
    }

    void BackEffect(Unit caster)
    {
        // Ganha Dodge
        Debug.Log($"{caster.unitName} Dodge antes = {caster.dodgeChance}"); 
        caster.AddStatus(new DodgeEffect
        {
            value = dodgeGain
        });
        Debug.Log($"{caster.unitName} Dodge depois = {caster.dodgeChance}");

        // Avança 2 posições
        BoardManager.Instance.TryMoveUnit(
            caster,
            advanceAmount,
            true
        );
    }
}