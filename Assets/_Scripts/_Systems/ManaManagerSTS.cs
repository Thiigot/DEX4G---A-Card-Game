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

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartTurn();
    }

    public void StartTurn()
    {
        currentMana = manaPerTurn;
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
            manaText.text = $"Mana: {currentMana}";
    }
}