using UnityEngine;
using CardData;
using System.Collections;

public abstract class CardSpecialEffect : ScriptableObject
{
    public virtual IEnumerator OnDraw(Unit owner, Card card) { yield break; }
    //public virtual void OnPlay(Unit caster, Unit target, Card card){ }
    public virtual IEnumerator OnPlayCoroutine(Unit caster,Unit target, Card card)
    {
        //OnPlay(caster, target, card);
        yield break;
    }

}