using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
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

    [Header("Hover Slot")]
    public Image slotImage;
    public float normalAlpha = 0.2f;
    public float hoverAlpha = 0.5f;

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();    
    }
    private void Start()
    {
        slotImage = GetComponent<Image>();
        //currentUnit = GetComponent<Unit>();
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
        if (TargetManager.Instance == null) return;

        if (TargetManager.Instance.IsTargeting())
        {
            // só aceita se tiver inimigo
            if (currentUnit != null)
            {
                TargetManager.Instance.SelectTarget(this);
            }
        }
    }


    #region HOVER
    public void SetHover(bool value)
    {

        if (!TargetManager.isTargeting)
            value = false;

        if (slotImage == null) return;

        Color c = slotImage.color;
        c.a = value ? hoverAlpha : normalAlpha;
        slotImage.color = c;

        if (currentUnit != null)
        {
            currentUnit.SetFlash(value);
        }
            
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (TargetManager.isTargeting) return;
        if (handManager != null && handManager.draggedCard != null) return; 

        if (currentUnit != null)
        {
            UnitInfoUI.Instance.Show(currentUnit, transform.position);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TargetManager.isTargeting) return;
        if (handManager != null && handManager.draggedCard != null) return;
        if (currentUnit != null)
        {
            UnitInfoUI.Instance.Hide();
        }
    }

    public void OnHoverEnter()
    {
        sr.color = Color.yellow;
    }

    public void OnHoverExit()
    {
        sr.color = Color.white;
    }

    #endregion
}