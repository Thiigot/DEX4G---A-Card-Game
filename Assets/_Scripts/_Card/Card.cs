using System.Collections.Generic;
using UnityEngine;


namespace CardData
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Card")] 
    public class Card : ScriptableObject
    {
        public string cardName;
        public CardType cardType;
        public int cardMana;
        public string textFront;
        public string textBack;
        public bool requiresTarget;
        public CardClass cardClass;
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

}
