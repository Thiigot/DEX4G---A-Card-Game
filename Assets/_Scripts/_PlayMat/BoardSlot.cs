using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardSlot : MonoBehaviour, IPointerClickHandler
{
    public bool isEnemy;
    public Unit currentUnit;
    public HandManager handManager;

    public SlotPosition slotPosition;
    public enum SlotPosition
    {
        Frontline,
        Backline
    }

    [Header("Visual")]
    public Image slotImage;
    public float normalAlpha = 0.2f;
    public float hoverAlpha = 0.5f;

    void Start()
    {
        slotImage = GetComponent<Image>();
    }

    public bool IsEmpty()
    {
        return currentUnit == null;
    }

    public void SetUnit(Unit unit)
    {
        currentUnit = unit;
        unit.SetSlot(this);
        unit.transform.SetParent(transform, false);

        RectTransform rt = unit.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;
        }
    }

    public void Clear()
    {
        currentUnit = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 🖱️ LEFT CLICK → TARGETING
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (TargetManager.Instance == null) return;

            if (TargetManager.Instance.IsTargeting())
            {
                if (currentUnit != null)
                {
                    TargetManager.Instance.SelectTarget(this);
                }
            }
        }

        // 🖱️ RIGHT CLICK → INFO PANEL
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (currentUnit == null) return;

            // 🔥 evita conflito com drag
            if (handManager != null && handManager.draggedCard != null)
                return;

            UnitInfoUI.Instance.Show(currentUnit, transform.position, isEnemy);
        }
    }

    public void SetTargetHighlight(bool value)
    {
        if (slotImage == null) return;

        Color c = slotImage.color;
        c.a = value ? hoverAlpha : normalAlpha;
        slotImage.color = c;

        if (currentUnit != null)
        {
            currentUnit.SetFlash(value);
        }
    }
}