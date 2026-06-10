using CardData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


////////////  ZEN BLADE
[CreateAssetMenu(menuName = "Wanderer/Zen Blade")]
public class ZenBladeEffect : CardSpecialEffect
{
    public int damage = 4;

    public override IEnumerator OnDraw( Unit owner, Card card)
    {
        yield return new WaitForSeconds(0.5f);

        List<Unit> enemies = new();

        foreach (var unit in FindObjectsByType<Unit>(FindObjectsSortMode.None))
        {
            if (unit.isPlayer != owner.isPlayer)
                enemies.Add(unit);
        }

        if (enemies.Count > 0)
        {
            Unit target = enemies[Random.Range(0, enemies.Count)];

            target.TakeDamage(
                owner.ModifyOutgoingDamage(damage),
                DamageType.Direct,
                owner
            );
        }
 
        yield return new WaitForSeconds(0.5f);
        owner.hand.Remove(card);
        owner.handManager.ShowHand(owner.hand);
        owner.deckManager.BanishCard(card, owner);

        yield return new WaitForSeconds(0.5f);
        owner.DrawCards(1);
    }
}
