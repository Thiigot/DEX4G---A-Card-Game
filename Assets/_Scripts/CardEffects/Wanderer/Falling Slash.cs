using CardData;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Wanderer/Falling Slash")]
public class FallingSlashEffect : CardSpecialEffect
{
    [SerializeField] private Card zenBladeCard;
    public int damage = 4;
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        if (caster.IsFrontline())
        {
            FrontEffect(caster, target, card);
        }
        else
        {
            BackEffect(caster, target, card);
        }
        yield break;
    }
    void FrontEffect(Unit caster, Unit target, Card card)
    {
        //DANO
        target.TakeDamage(
            caster.ModifyOutgoingDamage(damage),
            DamageType.Direct,
            caster
        );
        //RECUA 2
        BoardManager.Instance.TryMoveUnit(caster, 2, false);
        //ZB 2
        caster.deckManager.ZenBladeGen(2, caster);

    }
    void BackEffect(Unit caster, Unit target, Card card)
    {
        //DANO
        target.TakeDamage(
            caster.ModifyOutgoingDamage(damage),
            DamageType.Direct,
            caster
        );
        //DRAW 2
        caster.DrawCards(2);
        //ZB 2
        caster.deckManager.ZenBladeGen(2, caster);
    }
}
