using TMPro;
using UnityEngine;

public class UnitInfoUI : MonoBehaviour
{
    public static UnitInfoUI Instance;

    public GameObject panel;
    public TMP_Text hpText;
    public TMP_Text atkText;
    public TMP_Text manaText;
    public TMP_Text speedText;
    public TMP_Text nameText;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(Unit unit, Vector3 worldPos)
    {
        panel.SetActive(true);

        nameText.text = unit.unitClass;
        hpText.text = $"HP: {unit.currentHP} / {unit.maxHP}";
        atkText.text = $"ATK: {unit.attack}";
        manaText.text = $"MANA: {unit.currentMana} / {unit.maxMana}";
        speedText.text = $"SPD: {unit.speed}";

        panel.transform.position = new Vector3(worldPos.x +300,worldPos.y, 10f);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}