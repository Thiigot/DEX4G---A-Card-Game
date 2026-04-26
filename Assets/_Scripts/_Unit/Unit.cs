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
    public bool isPlayer;
    public bool hasStartedCombat = false;

    [Header("Flash variables")]
    private bool isFlashing = false;
    private float flashTimer = 0f;
    private float flashSpeed = 8f;

    [Header("UI")]
    public TMP_Text hpText;
    public Image spriteImage;
    private BoardSlot currentSlot;

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
        maxMana = data.baseMana;
        currentMana = maxMana;

        if (spriteImage != null)
            spriteImage.sprite = data.sprite;
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

    public void TakeDamage(int amount, DamageType type = DamageType.Direct)
    {
        int finalDamage = amount;

        foreach (var effect in activeEffects)
            effect.OnReceiveDamage(ref finalDamage, type);

        currentHP -= finalDamage;

        if (currentHP <= 0)
            Die();

        UpdateUI();
    }
    public int ModifyOutgoingDamage(int damage)
    {
        int finalDamage = damage;

        foreach (var effect in activeEffects)
            effect.OnDealDamage(ref finalDamage);

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
        ProcessTurnStart();

        if (isStunned)
        {
            Debug.Log($"{unitName} está stunado!");
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
    #endregion

    #region CARDS
    public void PlayCard(Card card, CardExecutionContext context, GameObject cardObj)
    {
        if (hand.Contains(card))
            hand.Remove(card);

        context.caster = this;
        context.isFront = IsFrontline();

        CardEffectExecutor.ExecuteCard(this,context.target, card);

        deckManager.AddToDiscard(card);
        deckManager.StartCoroutine(deckManager.AnimateDiscard(cardObj));
        deckUI.UpdateUI();
    }
    public void DrawCards(int amount)
    {
        StartCoroutine(DrawCardsAnimated(amount));
    }
    IEnumerator DrawCardsAnimated(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Card c = deckManager.DrawCard();

            if (c != null)
            {
                hand.Add(c);
                deckUI.UpdateUI();
                yield return StartCoroutine(
                    handManager.AnimateDrawCard(c, deckManager.deckPoint)
                );
                deckUI.UpdateUI();
                yield return new WaitForSeconds(0.1f); // delay entre cartas
            }
        }
    }
    void EndTurnCleanup()
    {
        handManager.ClearHandVisual();
    }
    #endregion

    #region STATUS SYSTEM
    public void AddStatus(StatusEffect effect)
    {
        if (effect == null) return;

        if (effect is StunEffect)
        {
            var existing = activeEffects.Find(e => e is StunEffect);
            if (existing != null)
            {
                // atualiza duração ao invés de adicionar
                existing.value = effect.value;
                return;
            }
        }

        effect.owner = this;
        activeEffects.Add(effect);
        effect.OnApply();

        UpdateStatusUI();
    }
    public void ProcessTurnStart()
    {
        foreach (var effect in activeEffects)
            effect.OnTurnStart();

        CleanupEffects();
    }
    public void ProcessTurnEnd()
    {
        foreach (var effect in activeEffects)
            effect.OnTurnEnd();

        CleanupEffects();
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
    void UpdateStatusUI()
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
            spriteImage.color = Color.white;
        }
    }
    #endregion

    void Update()
    {
        if (!isFlashing) return;

        flashTimer += Time.deltaTime * flashSpeed;

        float t = Mathf.Abs(Mathf.Sin(flashTimer));

        spriteImage.color = Color.Lerp(Color.white, new Color(2f,2f,2f,0f), t);

    }
}