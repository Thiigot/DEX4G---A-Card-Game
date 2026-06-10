using TMPro;
using UnityEngine;

public class ManaManagerSTS : MonoBehaviour
{
    public static ManaManagerSTS Instance;

    [Header("Mana")]
    public int currentMana;
    public int manaPerTurn = 3;

    [Header("UI")]
    public TMP_Text manaText;
    public Unit currentUnit;
    void Awake()
    {
        Instance = this;
    }
    public void SetCurrentUnit(Unit unit)
    {
        currentUnit = unit;
        UpdateUI();
    }

    void Start()
    {
        StartTurn();
    }

    public void StartTurn()
    {
        if(currentUnit == null)return;
        currentUnit.currentMana = currentUnit.maxMana;
        UpdateUI();
    }

    public bool HasEnoughMana(int cost)
    {
        return currentUnit!= null && currentUnit.currentMana >= cost;
    }

    public void SpendMana(int cost)
    {
        currentUnit.currentMana -= cost;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (currentUnit == null)
        {
            manaText.text = "Mana: 0";
            return;
        }
        manaText.text = $"Mana: {currentUnit.currentMana}/{currentUnit.maxMana}";
    }
}