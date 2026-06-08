using System.Collections.Generic;
using UnityEngine;
using CardData;

public static class CardEffectExecutor
{
    public static void ExecuteCard(Unit caster, Unit target, Card card)
    {
        // 🔥 verifica se está na frontline
        bool isFront = caster.IsFrontline();

        // 🔥 escolhe efeitos corretos
        List<CardEffect> effects = isFront ? card.effectsInFront : card.effectsInBack;

        foreach (var effect in effects)
        {
            List<Unit> targets = ResolveTargets(caster, target, effect);

            foreach (var t in targets)
            {
                ApplyEffect(caster, t, effect);
            }
        }
    }

    // 🔥 RESOLVE OS ALVOS CORRETAMENTE
    static List<Unit> ResolveTargets(Unit caster, Unit selectedTarget, CardEffect effect)
    {
        List<Unit> result = new List<Unit>();

        switch (effect.targetType)
        {
            case TargetType.Self:
                result.Add(caster);
                break;

            case TargetType.SingleEnemy:
                if (selectedTarget != null)
                    result.Add(selectedTarget);
                break;

            case TargetType.Ally:
                if (selectedTarget != null)
                    result.Add(selectedTarget);
                break;

            case TargetType.AllEnemies:
                result.AddRange(GetAllEnemies(caster));
                break;

            case TargetType.AllAllies:
                result.AddRange(GetAllAllies(caster));
                break;
        }

        return result;
    }

    // 🔥 APLICA EFEITOS
    static void ApplyEffect(Unit caster, Unit target, CardEffect effect)
    {
        switch (effect.effectType)
        {
            case EffectType.Damage:
                if (target != null)
                    target.TakeDamage(
                        caster.ModifyOutgoingDamage(effect.value),
                        DamageType.Direct,
                        caster,
                        effect.ignoreProtection
                        );
                break;

            case EffectType.Heal:
                caster.Heal(effect.value);
                break;

            case EffectType.Draw:
                caster.DrawCards(effect.value);
                break;

            case EffectType.Energy:
                caster.currentMana += effect.value;
                break;

            case EffectType.Speed:
                caster.speed += effect.value;
                break;
            case EffectType.Charge:
                MoveUnit(caster, effect.value, true);
                break;
            case EffectType.Retreat:
                MoveUnit(caster, effect.value, false);
                break;
            case EffectType.ZenBlade:
                HandleZenBlade(caster, effect);
                break;

            case EffectType.ApplyBleed:
                target.AddStatus(new BleedEffect
                {
                    value = effect.value,
                });
                break;

            case EffectType.ApplyStun:
                target.AddStatus(new StunEffect
                {
                    value = 1
                });
                break;

            case EffectType.ApplyMark:
                target.AddStatus(new MarkEffect
                {
                    value = effect.value
                });
                break;
            case EffectType.ApplyTaunt:
                target.AddStatus(new TauntEffect
                {
                    value = effect.value,
                    taunter = caster
                });
                break;
            case EffectType.ApplyStealth:
                target.AddStatus(new StealthEffect
                {
                    value = effect.value
                });
                break;
            case EffectType.ModifyWeakness:
                target.AddStatus(new WeaknessEffect
                {
                    value = effect.value
                });
                break;
            case EffectType.ModifyDodge:
                target.AddStatus(new DodgeEffect
                {
                    value = effect.value
                });
                break;
            case EffectType.Retaliate:
                target.AddStatus(new RetaliateEffect
                {
                    value = effect.value
                });
                break;
            case EffectType.ModifyProtection:
                target.AddStatus(new ProtectionEffect
                {
                    value = effect.value
                });
                break;
            case EffectType.ModifyDamage:
                target.AddStatus(new DamageModifierEffect
                {
                    value = effect.value
                });
                break;
        }
    }

    // 🔥 SISTEMAS AUXILIARES

    static void MoveUnit(Unit unit, int steps, bool forward)
    {
        BoardManager boardManager = BoardManager.Instance;

        if (boardManager == null)
            boardManager = GameObject.FindAnyObjectByType<BoardManager>();

        if (boardManager == null)
        {
            Debug.LogWarning("BoardManager não encontrado para mover unidade.");
            return;
        }

        boardManager.TryMoveUnit(unit, steps, forward);
    }

    static List<Unit> GetAllEnemies(Unit caster)
    {
        List<Unit> result = new List<Unit>();

        foreach (var unit in GameObject.FindObjectsOfType<Unit>())
        {
            if (unit.isPlayer != caster.isPlayer)
                result.Add(unit);
        }

        return result;
    }

    static List<Unit> GetAllAllies(Unit caster)
    {
        List<Unit> result = new List<Unit>();

        foreach (var unit in GameObject.FindObjectsOfType<Unit>())
        {
            if (unit.isPlayer == caster.isPlayer)
                result.Add(unit);
        }

        return result;
    }

    static void HandleZenBlade(Unit caster, CardEffect effect)
    {
        // FRONT → duplicar
        if (effect.value == 1)
        {
            List<Card> zenCards = caster.deckManager.GetAllZenBladeCards();

            foreach (var card in zenCards)
            {
                caster.deckManager.AddCardToDeck(card);
            }
        }
        // BACK → converter
        else if (effect.value == 2)
        {
            caster.deckManager.ConvertZenBladeToHeal();
        }
    }
}