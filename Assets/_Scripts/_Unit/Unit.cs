using CardData;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
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

    [Header("Runtime Stats")]
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

    void Update()
    {
        if (!isFlashing) return;

        flashTimer += Time.deltaTime * flashSpeed;

        float t = Mathf.Abs(Mathf.Sin(flashTimer));

        spriteImage.color = Color.Lerp(Color.white, new Color(2f,2f,2f,0f), t);

    }

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
            hpText.text= data.unitName;

        UpdateUI();
    }

    public void SetSlot(BoardSlot slot)
    {
        currentSlot = slot;
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;

        if (currentHP <= 0)
            Die();

        UpdateUI();
    }

    void Die()
    {
        if (currentSlot != null)
            currentSlot.Clear();

        Destroy(gameObject);
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

    public IEnumerator TakeTurn()
    {
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
        EndTurnCleanup();
    }

    IEnumerator EnemyTurn()
    {
        handManager.owner = this;
        deckUI.UpdateUI();
        yield return new WaitForSeconds(2f);
    }

    void DrawCards(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Card c = deckManager.DrawCard();

            if (c != null)
                hand.Add(c);
        }
    }

    void EndTurnCleanup()
    {
        handManager.ClearHandVisual();
    }

    public void PlayCard(Card card, GameObject cardObj)
    {
        if (hand.Contains(card))
            hand.Remove(card);

        //deckManager.AddToDiscard(card);

        deckManager.StartCoroutine(deckManager.AnimateDiscard(cardObj));
        deckUI.UpdateUI();
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

}