using System.Collections.Generic;
using UnityEngine;


namespace CardData
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Card")] 
    public class Card : ScriptableObject
    {
        public List<CardEffect> effectsInFront;
        public List<CardEffect> effectsInBack;


        public string cardName;
        public CardType cardType;
        public int cardMana;
        public string textInFront;
        public string textInBack;
        public bool requiresTargetInFront;
        public bool requiresTargetInBack;
        public CardClass cardClass;
        public bool isZenBlade;

        public List<CardEffect> GetEffects(Unit caster)
        {
            return caster.IsFrontline() ? effectsInFront : effectsInBack;
        }

        public bool RequiresTarget(Unit caster)
        {
            return caster.IsFrontline() ? requiresTargetInFront : requiresTargetInBack;
        }

        public string GetText(Unit caster)
        {
            return caster.IsFrontline() ? textInFront : textInBack;
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

    [System.Serializable]
    public class CardEffect
    {
        public EffectType effectType;

        public int value;

        public TargetType targetType;

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
        Execute,
        Repeat,
        Consume,

        // stats
        ModifyCrit,
        ModifyDodge,
        ModifyProtection,
        ModifyWeakness,

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
