using CardData;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Outlaw/QuickShot")]
public class QuickShotEffect : CardSpecialEffect
{
    public override IEnumerator OnPlayCoroutine(Unit caster, Unit target, Card card)
    {
        if (caster.IsFrontline())
        {
            for (int i = 0; i < 2; i++)
            {
                if (target == null)
                    break;

                target.TakeDamage(caster.ModifyOutgoingDamage(3), DamageType.Direct, caster);

                if (target.currentHP <= 0)
                    break;
            }
            yield return new WaitForSeconds(0.2f);
            target.AddStatus(new BleedEffect { value = 3 });
        }
        else
        {
            BoardManager.Instance.TryMoveUnit(caster, 2, true);
            yield return new WaitForSeconds(0.2f);
            target.AddStatus(new WeaknessEffect { value = 10 });
        }
        yield break;
    }
}