using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    [Header("Data")]
    public UnitData data;

    [Header("Runtime Stats")]
    public int currentHP;

    [Header("UI")]
    public TMP_Text hpText;
    public Image spriteImage;

    private BoardSlot currentSlot;

    public void Init(UnitData unitData)
    {
        data = unitData;

        currentHP = data.maxHP;

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
}