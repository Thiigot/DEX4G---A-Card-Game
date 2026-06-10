using CardData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Wanderer/Cruel Meditation")]
public class CruelMeditationEffect : CardSpecialEffect
{
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
        List<Card> zenBlades = FindAllZenBlades(caster);

        foreach (var card in zenBlades)
        {
            caster.deckManager.AddCardToDeck(card);
        }

        Debug.Log($"Cruel Meditation duplicou {zenBlades.Count} Zen Blades.");
    }
    void BackEffect(Unit caster)
    {
        List<Card> zenBlades = FindAllZenBlades(caster);
        foreach (var card in zenBlades)
        {
            caster.deckManager.BanishCard(card, caster);
            caster.Heal(2);
        }
    }
    List<Card> FindAllZenBlades(Unit caster)
    {
        List<Card> result = new();
        foreach (Card card in caster.deckManager.deck)
        {
            if (card.isZenBlade)
                result.Add(card);
        }
        return result;
    }
}


