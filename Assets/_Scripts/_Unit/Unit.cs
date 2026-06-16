using CardData;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using static BoardSlot;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;

public class Unit : MonoBehaviour
{
    [Header("Data")]
    public UnitData data;
    public HandManager handManager;
    public DeckManager deckManager;
    ManaManagerSTS manaManager;
    DeckUIManager deckUI;
    public List<Card> hand = new List<Card>();

    [Header("Runtime Effects")]
    public List<StatusEffect> activeEffects = new List<StatusEffect>();

    public bool isStunned;
    public bool isStealthed;
    public Unit tauntedBy;

    [Header("Stats")]
    public string unitName;
    public string unitClass;
    public int currentHP;
    public int attack;
    public int speed;
    public int currentMana;
    public int maxMana;
    public int maxHP;
    public float critChance;
    public float dodgeChance;
    public float retaliateChance;
    public float protection;
    public float weakness;
    public bool isPlayer;
    public bool hasStartedCombat = false;

    [Header("Turn Stats")]
    public int cardsDrawnThisTurn;
    public bool dodgedSinceLastTurn;
    public bool hasExtraTurn;
    public bool critAttack = false;
    public bool isChanneling;
    public System.Action channelResolveAction;

    [Header("Flash variables")]
    private bool isFlashing = false;
    private float flashTimer = 0f;
    private float flashSpeed = 8f;

    [Header("UI")]
    public TMP_Text hpText;
    public SpriteRenderer spriteRenderer;
    private BoardSlot currentSlot;
    public BoardSlot CurrentSlot => currentSlot;

    [Header("Status UI")]
    public Transform statusContainer;
    public GameObject statusIconPrefab;
    public StatusDatabase statusDatabase;

    private List<GameObject> spawnedIcons = new List<GameObject>();

    #region INIT
    private void Awake()
    {
        deckManager = GetComponent<DeckManager>();
        deckUI = FindAnyObjectByType<DeckUIManager>();
        manaManager = FindAnyObjectByType<ManaManagerSTS>();
        deckManager.deckPoint = deckUI.deckPoint;
        deckManager.discardPile = deckUI.discardPile;
        handManager = FindAnyObjectByType<HandManager>();
    }
    public void Init(UnitData unitData)
    {
        data = unitData;
        isPlayer = data.isPlayer;
        unitName = data.unitName;
        unitClass = data.unitName;
        maxHP = data.maxHP;
        currentHP = maxHP;
        attack = data.attack;
        speed = data.speed;
        critChance = data.critChance;
        dodgeChance = data.dodgeChance;
        retaliateChance = data.retaliateChance;
        protection = data.protection;
        weakness = data.weakness;
        maxMana = data.baseMana;
        currentMana = maxMana;

        if (spriteRenderer != null)
            spriteRenderer.sprite = data.sprite;
        if (hpText != null)
            hpText.text = data.unitName;

        UpdateUI();
    }
    #endregion

    #region POSITION
    public void SetSlot(BoardSlot slot)
    {
        currentSlot = slot;
    }
    public bool IsFrontline()
    {
        return currentSlot != null && currentSlot.slotPosition == SlotPosition.Frontline;
    }
    public bool IsBackline()
    {
        return currentSlot != null && currentSlot.slotPosition == SlotPosition.Backline;
    }
    public void MoveToSlot(BoardSlot newSlot)
    {
        if (currentSlot != null)
            currentSlot.Clear();

        newSlot.SetUnit(this);

        transform.position = newSlot.transform.position;
    }
 
    #endregion

    #region COMBAT
    public void Heal(int amount)
    {
        //if (currentHP + amount > maxHP)
        //    currentHP = maxHP;
        //else 
        currentHP += amount;

        UpdateUI();
    }
    public bool ConsumeNextAttackCrit()
    {
        NextAttackCritEffect effect = GetStatus<NextAttackCritEffect>();

        if (effect == null)
            return false;

        RemoveStatus<NextAttackCritEffect>();

        return true;
    }
    public int ConsumeNextAttackAdvance()
    {
        NextAttackAdvanceEffect effect = GetStatus<NextAttackAdvanceEffect>();

        if (effect == null)
            return 0;

        int amount = effect.value;

        RemoveStatus<NextAttackAdvanceEffect>();

        return amount;
    }
    public void TakeDamage(int amount, DamageType type = DamageType.Direct, Unit attacker = null, bool ignoreProtection = false, bool canTriggerRetaliate = true, bool isCritical = false)
    {
        isCritical = critAttack;
        if (isCritical && GetStatus<CritImmunityEffect>() != null)
        {
            isCritical = false;
        }
        if (attacker != null && type == DamageType.Direct)
        {
            int advance = attacker.ConsumeNextAttackAdvance();

            if (advance > 0)
            {
                BoardManager.Instance.TryMoveUnit(attacker,advance,true);
            }
        }
        if (TryDodge())
        {
            Debug.Log($"{unitName} dodged the attack!");

            if(canTriggerRetaliate && attacker != null)
                TryRetaliate(attacker);
            
            return;
        }

        int finalDamage = amount;
        if (isCritical)
        {
            finalDamage *= 2;
        }
        bool shouldIgnoreProtection = ignoreProtection || (attacker != null && attacker.HasIgnoreProtection());

        if (!shouldIgnoreProtection)
        {
            finalDamage = Mathf.RoundToInt(finalDamage * (1f - protection / 100f));
            foreach (var effect in activeEffects)
                effect.OnReceiveDamage(ref finalDamage, type);
        }

        finalDamage = Mathf.Max(0, finalDamage);
        currentHP -= finalDamage;

        foreach (var effect in activeEffects)
        {
            effect.OnOwnerDamaged(
                attacker,
                finalDamage
            );
        }

        if (currentHP <= 0)
            Die();

        if (currentHP > 0 && canTriggerRetaliate && attacker != null)
            TryRetaliate(attacker);

        UpdateUI();
    }
    public int ModifyOutgoingDamage(int damage, bool allowCrit = true, DamageType type = DamageType.Direct)
    {
        int finalDamage = damage;

        if(weakness > 0f)
            finalDamage = Mathf.RoundToInt(damage * (1f - weakness / 100f));

        foreach (var effect in activeEffects)
            effect.OnDealDamage(ref finalDamage);

        bool forceCrit = false;

        if (type == DamageType.Direct)
        {
            forceCrit = ConsumeNextAttackCrit();
        }
        bool isCrit = allowCrit && (forceCrit || TryCrit());

        if (isCrit)
        {
            finalDamage *= 2;
            critAttack = true;
            Debug.Log($"{unitName} CRITICAL HIT!");
        }
        if (!isCrit)
        {
            critAttack = false;
        }
        return finalDamage;
    }
    void Die()
    {
        if (currentSlot != null)
            currentSlot.Clear();

        Destroy(gameObject);
    }
    #endregion

    #region TURN
    public IEnumerator TakeTurn()
    {
        cardsDrawnThisTurn = 0;
        ProcessTurnStart();

        if (isStunned || isChanneling)
        {
            ProcessTurnEnd();
            yield break;
        }

        if (isPlayer)
            yield return PlayerTurn();
        else
            yield return EnemyTurn();
    }
    IEnumerator PlayerTurn()
    {
        handManager.owner = this;
        manaManager.SetCurrentUnit(this);
        TurnManager.Instance.playerFinishedTurn = false;
        manaManager.StartTurn();
        if (!hasStartedCombat)
        {
            deckManager.Init(this, handManager);
            hand.Clear();
            deckUI.SetCurrentDeck(deckManager, handManager);
            deckUI.UpdateUI();
            yield return StartCoroutine(DrawCardsAnimated(5));
            hasStartedCombat = true;
        }
        else
        {
            handManager.ShowHand(hand);
            yield return new WaitForSeconds(0.5f);
            deckUI.SetCurrentDeck(deckManager, handManager);
            deckUI.UpdateUI();
            yield return StartCoroutine(DrawCardsAnimated(1));
        }
        deckUI.UpdateUI();
        yield return new WaitUntil(() => TurnManager.Instance.playerFinishedTurn);
        ProcessTurnEnd();
        EndTurnCleanup();
    }
    IEnumerator EnemyTurn()
    {
        handManager.owner = this;
        deckUI.UpdateUI();
        yield return new WaitForSeconds(2f);
        ProcessTurnEnd();
    }
    public void ProcessTurnStart()
    {
        foreach (var effect in activeEffects)
            effect.OnTurnStart();

        CleanupEffects();

        if(handManager != null)
            handManager.RefreshAllCardTexts();
    }
    public void ProcessTurnEnd()
    {
        foreach (var effect in activeEffects)
            effect.OnTurnEnd();

        CleanupEffects();
        dodgedSinceLastTurn = false;
        deckManager.ResetTemporaryCosts();
        handManager.RefreshHandVisual();
        if (handManager != null)
            handManager.RefreshAllCardTexts();
    }
    #endregion

    #region CARDS
    public IEnumerator PlayCard(Card card, CardExecutionContext context, GameObject cardObj)
    {
        if (hand.Contains(card))
            hand.Remove(card);

        context.caster = this;
        context.isFront = IsFrontline();

        yield return CardEffectExecutor.ExecuteCardCoroutine(this, context.target, card);

        handManager.RefreshAllCardTexts();

        if (card.shuffleIntoDeckInsteadOfDiscard)
        {
            card.shuffleIntoDeckInsteadOfDiscard = false;
            deckManager.deck.Add(card);
            deckManager.ShuffleDeck();
        }
        else
        {
            deckManager.AddToDiscard(card);
            yield return deckManager.AnimateDiscard(cardObj);
            deckUI.UpdateUI();
        }
    }

    public IEnumerator PlayCardFree(Card card)
    {
        Unit autoTarget =
            CardEffectExecutor.GetAutomaticTarget(
                this,
                card
            );

        yield return CardEffectExecutor.ExecuteCardCoroutine(
            this,
            autoTarget,
            card
        );

        deckManager.AddToDiscard(card);
    }

    public void DrawCards(int amount)
    {
        StartCoroutine(DrawCardsAnimated(amount));
    }
    public IEnumerator DrawCardsAnimatedPublic(int amount)
    {
        yield return StartCoroutine(
            DrawCardsAnimated(amount)
        );
    }

    IEnumerator DrawCardsAnimated(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Card c = deckManager.DrawCard();

            if (c != null)
            {
                hand.Add(c);
                cardsDrawnThisTurn++;

                yield return StartCoroutine(handManager.AnimateDrawCard(c,deckManager.deckPoint));
                CardSpecialEffect effect = c.specialEffect;
                if(c.specialEffect != null)
                    yield return StartCoroutine(effect.OnDraw(this, c));

                deckUI.UpdateUI();
                yield return new WaitForSeconds(0.1f); // delay entre cartas
            }
        }
    }
    void EndTurnCleanup()
    {
        handManager.ClearHandVisual();
    }
    public IEnumerator DrawSingleCard(System.Action<Card> onCardDrawn)
    {
        Card c = deckManager.DrawCard();

        if (c == null)
        {
            onCardDrawn?.Invoke(null);
            yield break;
        }

        hand.Add(c);
        cardsDrawnThisTurn++;

        yield return StartCoroutine(
            handManager.AnimateDrawCard(
                c,
                deckManager.deckPoint
            )
        );

        if (c.specialEffect != null)
        {
            yield return StartCoroutine(
                c.specialEffect.OnDraw(this, c)
            );
        }

        deckUI.UpdateUI();

        onCardDrawn?.Invoke(c);
    }
    #endregion

    #region STATUS SYSTEM
    public bool IsTauntingSomeone()
    {
        List<Unit> enemies =
            CardEffectExecutor.GetAllEnemies(this);

        foreach (Unit enemy in enemies)
        {
            if (enemy.tauntedBy == this)
                return true;
        }

        return false;
    }
    public T GetStatus<T>() where T : StatusEffect
    {
        foreach (var effect in activeEffects)
        {
            if (effect is T typed)
                return typed;
        }

        return null;
    }

    public void RemoveStatus<T>() where T : StatusEffect
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            if (activeEffects[i] is T)
            {
                activeEffects[i].OnExpire();
                activeEffects.RemoveAt(i);
            }
        }

        UpdateStatusUI();
    }

    public void ApplyBleed(Unit target, int amount)
    {
        int finalBleed = amount;

        WolfBleedEffect wolf =
            GetStatus<WolfBleedEffect>();

        if (wolf != null)
        {
            finalBleed *= 2;

            RemoveStatus<WolfBleedEffect>();
        }

        target.AddStatus(
            new BleedEffect()
            {
                value = finalBleed
            }
        );
    }
    public int GetBleedStacks()
    {
        foreach (var effect in activeEffects)
        {
            if (effect is BleedEffect bleed)
                return bleed.value;
        }

        return 0;
    }
    public bool HasIgnoreProtection()
    {
        return activeEffects.Exists(
        e => e is IgnoreProtectionEffect
    );
    }
    public bool TryCrit()
    {
        float chance = critChance;
        foreach(var effect in activeEffects)
            effect.ModifyCritChance(ref chance);
        chance = Mathf.Clamp(chance, 0f, 100f);

        return chance > 0f && Random.value * 100f < chance;
    }
    bool TryDodge()
    {
        float chance = dodgeChance;

        foreach (var effect in activeEffects)
            effect.ModifyDodgeChance(ref chance);

        chance = Mathf.Clamp(chance, 0f, 100f);

        bool dodged =
            chance > 0f &&
            Random.value * 100f < chance;

        if (dodged)
            dodgedSinceLastTurn = true;

        return dodged;
    }
    void TryRetaliate(Unit attacker)
    {
        if (attacker == null) return;

        float chance = retaliateChance;
        foreach (var effect in activeEffects)
            effect.ModifyRetaliateChance(ref chance);
        chance = Mathf.Clamp(chance, 0f, 100f);
        if (chance <= 0f || Random.value * 100f >= chance) return;
        int retaliateDamage = Mathf.Max(1, ModifyOutgoingDamage(attack));
        Debug.Log($"{unitName} retaliates against {attacker.unitName} for {retaliateDamage} damage!");
        attacker.TakeDamage(retaliateDamage, DamageType.Direct, this, false, false);
    }
    public void AddStatus(StatusEffect neweffect)
    {
        if (neweffect == null) return;
        if (neweffect.IsDebuff())
        {
            DebuffImmunityEffect immunity = GetStatus<DebuffImmunityEffect>();
            if(immunity != null)
            {
                Debug.Log($"{unitName} ESTÁ IMUNE A DEBUFFs!");
                return;
            }
        }
        if (neweffect is StunEffect)
        {
            var existing = activeEffects.Find(e => e is StunEffect);
            if (existing != null)
            {
                // atualiza duração ao invés de adicionar
                existing.value = neweffect.value;
                UpdateStatusUI();
                return;
            }
        }
        foreach (var effect in activeEffects)
        {
            if (effect.GetTypeID() == neweffect.GetTypeID())
            {
                if(effect is DodgeEffect)
                {
                    effect.value += neweffect.value;
                    dodgeChance += neweffect.value;
                    UpdateStatusUI();
                    return;
                }
                if (effect is CritEffect)
                {
                    effect.value += neweffect.value;
                    critChance += neweffect.value;

                    UpdateStatusUI();
                    return;
                }
                // 🔥 TAUNT atualiza valor
                if (effect is TauntEffect taunt &&
                    neweffect is TauntEffect newTaunt)
                {
                    taunt.value = newTaunt.value;
                    taunt.taunter = newTaunt.taunter;
                    UpdateStatusUI();
                    return;
                }

                // 🔥 STATUS STACKÁVEIS
                if (effect.IsStackable())
                {
                    effect.OnStack(neweffect.value);
                }
                else
                {
                    effect.value = Mathf.Max(effect.value, neweffect.value);
                }
                UpdateStatusUI();
                return;
            }
        }

        neweffect.owner = this;
        activeEffects.Add(neweffect);
        neweffect.OnApply();

        UpdateStatusUI();
    }
    void CleanupEffects()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            if (activeEffects[i].IsExpired())
            {
                activeEffects[i].OnExpire();
                activeEffects.RemoveAt(i);
            }
        }

        UpdateStatusUI();
    }
    #endregion

    #region UI
    public void UpdateStatusUI()
    {
        // limpa antigos
        foreach (var obj in spawnedIcons)
            Destroy(obj);

        spawnedIcons.Clear();

        // cria novos
        foreach (var effect in activeEffects)
        {
            GameObject iconObj = Instantiate(statusIconPrefab, statusContainer);

            StatusIconUI ui = iconObj.GetComponent<StatusIconUI>();

            Sprite icon = statusDatabase.GetIcon(effect.GetTypeID());

            ui.Setup(icon, effect);

            spawnedIcons.Add(iconObj);
        }
    }
    void UpdateUI()
    {
        if (hpText != null)
            hpText.text = currentHP + "/" + data.maxHP;
    }
    public void SetFlash(bool value)
    {
        isFlashing = value;

        if (!value)
        {
            flashTimer = 0f;
            spriteRenderer.color = Color.white;
        }
    }
    #endregion

    void Update()
    {
        if (!isFlashing) return;

        flashTimer += Time.deltaTime * flashSpeed;

        float t = Mathf.Abs(Mathf.Sin(flashTimer));

        spriteRenderer.color = Color.Lerp(Color.white, new Color(2f,2f,2f,0f), t);

    }
}