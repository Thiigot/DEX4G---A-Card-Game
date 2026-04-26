using UnityEngine;

public abstract class StatusEffect
{
    public Unit owner;
    public int value;

    public abstract StatusType GetTypeID();
    public virtual void OnApply() { }
    public virtual void OnTurnStart() { }
    public virtual void OnTurnEnd() { }
    public virtual bool ShowValue() => true;
    public virtual void OnReceiveDamage(ref int damage, DamageType type) { }
    public virtual void OnDealDamage(ref int damage) { }
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

    public override void OnTurnEnd()
    {
        value--;
    }
}

//////////  TAUNT  //////////
public class TauntEffect : StatusEffect
{
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
