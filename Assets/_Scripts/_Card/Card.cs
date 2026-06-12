using System.Collections.Generic;
using UnityEngine;


namespace CardData
{
    [CreateAssetMenu(menuName = "Card/New Card")] 
    public class Card : ScriptableObject
    {
        [Header("Effects")]
        public CardSpecialEffect specialEffect;

        [HideInInspector]
        public int temporaryManaCost = -1;

        [Header("Traits")]
        public string cardName;
        public CardType cardType;
        public CardClass cardClass;
        public int cardMana;
        public bool isXCost;
        public string textInFront;
        public string textInBack;

        [Header("Details")]
        public bool requiresTargetInFront;
        public bool requiresTargetInBack;
        public bool isZenBlade;

        public bool RequiresTarget(Unit caster)
        {
            return caster.IsFrontline() ? requiresTargetInFront : requiresTargetInBack;
        }

        public string GetText(Unit caster)
        {
            return caster.IsFrontline() ? textInFront : textInBack;
        }

        public void SetTemporaryCost(int cost)
        {
            temporaryManaCost = cost;
        }
        public void ClearTemporaryCost()
        {
            temporaryManaCost = -1;
        }
        public int GetCurrentCost()
        {
            if (temporaryManaCost >= 0)
                return temporaryManaCost;

            return cardMana;
        }
    }
    public enum CardType
    {
        Attack,
        Skill,
        Construct,
        Stance
    }
    public enum CardClass 
    {
        Jackpot,
        Outlaw,
        Captain,
        Wanderer,
        Mechanic,
        Jumper
    }

    public enum EffectType
    {
        // básicos
        Damage,
        Heal,
        Draw,
        Discard,
        Energy,
        Speed,

        // status
        ApplyBleed,
        ApplyMark,
        ApplyStun,
        ApplyStealth,
        ApplyTaunt,
        ApplyLifesteal,
        ApplyChannel,

        // especiais
        Retaliate,
        Execute,
        Repeat,
        Consume,

        // stats
        ModifyCrit,
        ModifyDodge,
        ModifyProtection,
        ModifyWeakness,
        ModifyDamage,

        // jackpot
        Dice,
        Coin,
        AddChip,
        UseChip,
        AddLuck,

        // classe
        ZenBlade,
        Scrap,
        Exhaust,

        // movimento
        Charge,
        Retreat
    }

    public enum TargetType
    {
        Self,
        SingleEnemy,
        AllEnemies,
        Ally,
        AllAllies
    }
}
