using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class Unit : MonoBehaviour
{
    [Header("Data")]
    public UnitData data;


    [Header("Runtime Stats")]
    public string cardName;
    public int currentHP;
    public int attack;
    public int speed;
    public int currentMana;
    public int maxMana;
    public int maxHP;

    [Header("Flash variables")]
    private bool isFlashing = false;
    private float flashTimer = 0f;
    private float flashSpeed = 8f;

    [Header("UI")]
    public TMP_Text hpText;
    public Image spriteImage;
    public GameObject infoPanel;
    private BoardSlot currentSlot;

    void Update()
    {
        if (!isFlashing) return;

        flashTimer += Time.deltaTime * flashSpeed;

        float t = Mathf.Abs(Mathf.Sin(flashTimer));

        spriteImage.color = Color.Lerp(Color.white, new Color(2f,2f,2f,0f), t);
    }

    public void Init(UnitData unitData)
    {
        data = unitData;
        cardName = data.unitName;
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

}