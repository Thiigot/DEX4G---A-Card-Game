using UnityEngine;
using UnityEngine.UI;

public class PlayArea : MonoBehaviour
{
    public static PlayArea Instance;
    public GameObject highlight;

    private HandManager handManager;
    private Canvas canvas;
    private Image img;

    public static bool HasCardInPlay = false;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        handManager = FindAnyObjectByType<HandManager>();
        canvas = GetComponentInParent<Canvas>();
        img = highlight.GetComponent<Image>();
        img.color = new Color(1, 1, 1, 0);
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
                rect.position,
                null
            );

            img.color = new Color(1, 1, 1, 0.2f);

            if (img != null)
            {
                float targetAlpha = inside ? 0.8f : 0.2f;

                Color c = img.color;
                c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * 10f);
                img.color = c;
            }
        }
        else
        {
            img.color = new Color(1, 1, 1, 0);
        }
    }

    public bool IsInside(RectTransform cardRect)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
        transform as RectTransform,
        cardRect.position,
        null
        );
    }
}