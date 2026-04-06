using UnityEngine;
using UnityEngine.EventSystems;

public class BoardSlot : MonoBehaviour, IPointerClickHandler
{
    public bool isEnemy;
    public Unit currentUnit;

    public bool IsEmpty()
    {
        return currentUnit == null;
    }

    public void SetUnit(Unit unit)
    {
        currentUnit = unit;

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
}