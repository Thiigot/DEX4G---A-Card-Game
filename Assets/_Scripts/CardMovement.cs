using UnityEngine;
using UnityEngine.EventSystems;

public class CardMovement : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private Vector3 basePosition;
    private Vector2 dragOffset;

    private HandManager handManager;
    private DeckManager deckManager;
    private PlayArea playArea;

    private bool isDragging;
    private bool isHovering;

    [Header("Hover")]
    public float hoverScale = 1.2f;
    public float hoverHeight = 80f;
    public GameObject glow;



    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Update()
    {
        if (isDragging) return;

        Vector3 target = basePosition;

        if (isHovering)
            target += Vector3.up * hoverHeight;

        rectTransform.localPosition = Vector3.Lerp(
            rectTransform.localPosition,
            target,
            Time.deltaTime * 10f
        );

        float scale = isHovering ? hoverScale : 1f;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            Vector3.one * scale,
            Time.deltaTime * 10f
        );
    }

    public void SetHandManager(HandManager manager)
    {
        handManager = manager;
        canvas = manager.GetComponentInParent<Canvas>();
        deckManager = FindAnyObjectByType<DeckManager>();
        playArea = FindAnyObjectByType<PlayArea>();
    }

    public void SetBasePosition(Vector3 pos)
    {
        basePosition = pos;
    }

    #region HOVER
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging || handManager.draggedCard != null) return;

        isHovering = true;

        if (glow) glow.SetActive(true);

        handManager.hoveredCard = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDragging || handManager.draggedCard != null) return;

        isHovering = false;

        if (glow) glow.SetActive(false);

        if (handManager.hoveredCard == this)
            handManager.hoveredCard = null;
    }
    #endregion

    #region DRAG
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        isHovering = false;

        handManager.draggedCard = this;
        handManager.hoveredCard = null;

        transform.SetParent(canvas.transform);
        transform.rotation = Quaternion.identity;

        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out mousePos
        );

        dragOffset = mousePos - (Vector2)rectTransform.localPosition;

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pos
        );

        rectTransform.localPosition = pos - dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        bool played = playArea.IsInside(rectTransform);

        if (played)
        {
            // 🔥 CARTA JOGADA
            deckManager.DiscardCard(gameObject);
        }
        else
        {
            // 🔁 VOLTA PRA MÃO
            transform.SetParent(handManager.transform);
        }

        handManager.draggedCard = null;
        canvasGroup.blocksRaycasts = true;
    }
    #endregion
}