using UnityEngine;

public class PlayArea : MonoBehaviour
{
    public static PlayArea Instance;
    public GameObject highlight;

    private HandManager handManager;
    private Canvas canvas;

    void Awake()
    {
        Instance = this;
    }
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
                rect.position,
                null
            );

            // 🔥 sempre visível durante drag
            highlight.SetActive(true);

            // 💡 opcional: feedback visual
            var img = highlight.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
            {
                float targetAlpha = inside ? 0.5f : 0.2f;

                Color c = img.color;
                c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * 10f);
                img.color = c;
            }
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
        cardRect.position,
        null
        );
    }
}