using UnityEngine;

public class PlayArea : MonoBehaviour
{
    public GameObject highlight;

    private HandManager handManager;
    private Canvas canvas;

    void Start()
    {
        handManager = FindAnyObjectByType<HandManager>();
        canvas = GetComponentInParent<Canvas>();

        if (highlight != null)
            highlight.SetActive(false);
    }

    void Update()
    {
        if (highlight == null || handManager == null) return;

        var dragged = handManager.draggedCard;

        if (dragged != null)
        {
            RectTransform rect = dragged.GetComponent<RectTransform>();

            bool inside = RectTransformUtility.RectangleContainsScreenPoint(
                transform as RectTransform,
                RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, rect.position),
                canvas.worldCamera
            );

            highlight.SetActive(inside);
        }
        else
        {
            highlight.SetActive(false);
        }
    }

    public bool IsInside(RectTransform cardRect)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            transform as RectTransform,
            RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, cardRect.position),
            canvas.worldCamera
        );
    }
}