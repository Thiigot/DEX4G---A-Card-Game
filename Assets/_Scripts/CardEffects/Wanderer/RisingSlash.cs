using UnityEngine;
using CardData;
using System.Collections;

[CreateAssetMenu(menuName = "Wanderer/Rising Slash")]
public class RisingSlashEffect : CardSpecialEffect
{
    [Header("Stats")]
    public int damage = 4;

    public override IEnumerator OnPlayCoroutine(
        Unit caster,
        Unit target,
        Card card)
    {
        //------------------------------------------------
        // ATAQUE
        //------------------------------------------------

        if (target != null)
        {
            target.TakeDamage(
                caster.ModifyOutgoingDamage(damage),
                DamageType.Direct,
                caster
            );
        }

        yield return new WaitForSeconds(0.2f);

        //------------------------------------------------
        // FRONT / BACK
        //------------------------------------------------

        //FRONT
        if (caster.IsFrontline())
        {
            // Compra 2
            caster.DrawCards(2);
        }
        //BACK
        else
        {
            // Charge 2

            BoardManager.Instance.TryMoveUnit(
                caster,
                2,
                true
            );
        }

        yield return new WaitForSeconds(0.2f);

        //------------------------------------------------
        // CRIA 2 ZEN BLADES
        //------------------------------------------------

        caster.deckManager.ZenBladeGen(2, caster);
    }
}