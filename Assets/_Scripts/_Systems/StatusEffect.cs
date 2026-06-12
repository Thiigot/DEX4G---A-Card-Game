using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public abstract class StatusEffect
{
    public Unit owner;
    public int value;

    public abstract StatusType GetTypeID();
    public virtual bool IsStackable() => true;
    public virtual void OnStack(int addedValue) { }
    public virtual void OnApply() { }
    public virtual void OnTurnStart() { }
    public virtual void OnTurnEnd() { }
    public virtual bool ShowValue() => true;
    public virtual void OnReceiveDamage(ref int damage, DamageType type) { }
    public virtual void OnDealDamage(ref int damage) { }
    public virtual void ModifyDodgeChance(ref float chance) { }
    public virtual void ModifyRetaliateChance(ref float chance) { }
    public virtual void ModifyCritChance(ref float chance) { }
    public virtual void ModifySpeed(ref int speed) { }
    public virtual void OnExpire() { }

    public virtual bool IsExpired()
    {
        return value <= 0;
    }
}

////////////  BLEED  ///////////
public class BleedEffect : StatusEffect
{
    public override StatusType GetTypeID() => StatusType.Bleed;
    public override void OnTurnStart()
    {
        owner.TakeDamage(value, DamageType.DoT);
        value--;
    }
}

////////////  STUN  ////////////
public class StunEffect : StatusEffect
{
    public override bool ShowValue() => false;
    public override StatusType GetTypeID() => StatusType.Stun;
    public override bool IsStackable() => false;
    public override void OnApply()
    {
        owner.isStunned = true;
    }
    public override void OnTurnEnd()
    {
        value--;
        if (value <= 0)
        {
            owner.isStunned = false;
        }
    }

    public override void OnExpire()
    {
        owner.isStunned = false;
    }


}

////////////  MARK  ////////////
public class MarkEffect : StatusEffect
{
    public override StatusType GetTypeID() => StatusType.Mark;
    public override void OnReceiveDamage(ref int damage, DamageType type)
    {
        if(type != DamageType.Direct) return;
        damage = Mathf.RoundToInt(damage * 1.25f);
    }
    public override void OnTurnStart()
    {
        value--;
    }
}

///////// PROTECTION  /////////
public class ProtectionEffect : StatusEffect
{
    public override StatusType GetTypeID() => StatusType.Protection;
    public override void OnReceiveDamage(ref int damage, DamageType type)
    {
        if (type != DamageType.Direct) return;
        damage = Mathf.RoundToInt(damage * (1f - value / 100f));
    }
}

/////////  WEAKNESS  /////////
public class WeaknessEffect : StatusEffect
{
    public override StatusType GetTypeID() => StatusType.Weakness;
    public override void OnDealDamage(ref int damage)
    {
        damage = Mathf.RoundToInt(damage * (1f - value / 100f));
    }
}

/////////  DAMAGEMODIFIER  /////////
public class DamageModifierEffect : StatusEffect
{
    public override bool ShowValue() => false;
    public override StatusType GetTypeID() => StatusType.DamageModifier;
    public override void OnDealDamage(ref int damage)
    {
        damage = Mathf.RoundToInt(damage * (1f + value / 100f));
    }
}
/////////  STEALTH  /////////
public class StealthEffect : StatusEffect
{
    public override StatusType GetTypeID() => StatusType.Stealth;
    public override void OnApply()
    {
        owner.isStealthed = true;
    }

    public override void OnExpire()
    {
        owner.isStealthed = false;
    }

    public override void OnTurnStart()
    {
        value--;
    }
}

//////////  TAUNT  //////////
public class TauntEffect : StatusEffect
{
    public override bool ShowValue() => false;
    public override StatusType GetTypeID() => StatusType.Taunt;

    public Unit taunter;

    public override void OnApply()
    {
        owner.tauntedBy = taunter;
    }

    public override void OnExpire()
    {
        owner.tauntedBy = null;
    }

    public override void OnTurnEnd()
    {
        value--;
    }
}

//////////  DODGE  //////////
public class DodgeEffect : StatusEffect
{
    public override bool ShowValue() => false;
    public override StatusType GetTypeID() => StatusType.Dodge;
    public override bool IsStackable() => true;
    public override void OnStack(int addedValue)
    {
        value += addedValue;
        owner.dodgeChance += addedValue;
    }
    public override void OnApply()
    {
        owner.dodgeChance += value;
    }

    public override void OnTurnEnd()
    {
        int decay = 5;

        owner.dodgeChance -= decay;
        value -= decay;
    }
    public override void OnExpire()
    {
        owner.dodgeChance -= value;
        if(owner.dodgeChance < 0)
            owner.dodgeChance = 0;

    }
}

//////////  RETALIATE  //////////
public class RetaliateEffect : StatusEffect
{
    public override StatusType GetTypeID() => StatusType.Retaliate;

    public override void ModifyRetaliateChance(ref float chance)
    {
        chance += value;
    }

}

//////////  CRIT  //////////
public class CritEffect : StatusEffect
{
    public override bool ShowValue() => false;
    public override StatusType GetTypeID()
        => StatusType.Crit;

    public override bool IsStackable()
        => true;
    public override void OnStack(int addedValue)
    {
        value += addedValue;
        owner.critChance += addedValue;
    }
    public override void OnApply()
    {
        owner.critChance += value;
    }
    public override void ModifyCritChance(ref float chance)
    {
        chance += value;
    }

    public override void OnTurnEnd()
    {
        value -= 10;

        if (value < 0)
            value = 0;
    }

    public override bool IsExpired()
    {
        return value <= 0;
    }
}


////////////// IGNORE PROTECTION //////////
public class IgnoreProtectionEffect : StatusEffect
{
    public override bool ShowValue() => false;
    public override StatusType GetTypeID() => StatusType.IgnoreProtection;
    public override bool IsStackable() => false;
    public override void OnTurnEnd()
    {
        value--;
    }
}

////////////// NEXTATTACKBONUS ////////////
public class NextAttackBonusEffect : StatusEffect
{
    public float multiplier = 1f;
    bool consumed;
    public override StatusType GetTypeID()=> StatusType.NextDamage;

    public override void OnDealDamage(ref int damage)
    {
        if (consumed) return;
        damage = Mathf.RoundToInt(damage * multiplier);

        consumed = true;
        value = 0; // consome o efeito
    }

    public override bool IsExpired()
    {
        return value <= 0;
    }
}