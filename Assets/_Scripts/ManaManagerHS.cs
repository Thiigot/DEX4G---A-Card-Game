using TMPro;
using UnityEngine;

public class ManaManagerHS : MonoBehaviour
{
    [Header("Mana")]
    public int currentMana;
    public int maxMana;
    public int maxManaCap = 10;

    [Header("UI")]
    public TMP_Text manaText;

    void Start()
    {
        StartTurn();
    }

    public void StartTurn()
    {
        if (maxMana < maxManaCap)
            maxMana++;

        currentMana = maxMana;

        UpdateUI();
    }

    public bool HasEnoughMana(int cost)
    {
        return currentMana >= cost;
    }

    public void SpendMana(int cost)
    {
        currentMana -= cost;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (manaText != null)
            manaText.text = $"Mana: {currentMana} / {maxMana}";
    }
}