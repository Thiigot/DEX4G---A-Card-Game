using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UnitInfoUI : MonoBehaviour
{
    public static UnitInfoUI Instance;

    [Header("UI")]
    public GameObject panel;
    public TMP_Text hpText;
    public TMP_Text atkText;
    public TMP_Text manaText;
    public TMP_Text speedText;
    public TMP_Text nameText;

    [Header("Canvas Ref")]
    public RectTransform canvasRect;

    [Header("Status UI")]
    public Transform statusContainer;
    public GameObject statusIconPrefab;
    public StatusDatabase statusDatabase;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(Unit unit, Vector3 worldPos, bool isEnemy)
    {
        if (unit == null) return;

        panel.SetActive(true);

        // 🔥 TEXTOS
        nameText.text = unit.unitClass;
        hpText.text = $"HP: {unit.currentHP} / {unit.maxHP}";
        atkText.text = $"ATK: {unit.attack}";
        manaText.text = $"MANA: {unit.currentMana} / {unit.maxMana}";
        speedText.text = $"SPD: {unit.speed}";

        // 🔥 WORLD → SCREEN
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            null,
            out localPoint
        );

        UpdateStatusIcons(unit);

        Vector2 finalPos = localPoint;
        finalPos.x = isEnemy ? -390f : 390f;
        finalPos.y = 0f;
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchoredPosition = finalPos;
    }

    void UpdateStatusIcons(Unit unit)
    {
        // 🔥 limpa antigos
        foreach (Transform child in statusContainer)
            Destroy(child.gameObject);

        // 🔥 cria novos
        foreach (var effect in unit.activeEffects)
        {
            GameObject obj = Instantiate(statusIconPrefab, statusContainer);

            //var icon = obj.GetComponent<StatsIconUI2>();
            //icon.Setup(effect, statusDatabase);

            var icon = obj.GetComponent<StatusIconUI>();
            Sprite iconSprite = statusDatabase.GetIcon(effect.GetTypeID());
            icon.Setup(iconSprite, effect);
        }
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    void Update()
    {
        if (!panel.activeSelf) return;

        // 🖱️ CLICK (qualquer botão)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame ||
            Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            // 🔥 se clicou fora do painel
            if (!IsPointerOverPanel())
            {
                Hide();
                return;
            }
        }

        // ⌨️ ESC
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Hide();
        }
    }
    bool IsPointerOverPanel()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Mouse.current.position.ReadValue();

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject == panel ||
                result.gameObject.transform.IsChildOf(panel.transform))
            {
                return true;
            }
        }

        return false;
    }
}