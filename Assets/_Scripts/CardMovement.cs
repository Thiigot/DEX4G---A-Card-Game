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

    [Header("DragMovement")]
    private bool hasLeftHand = false;
    private Transform originalParent;
    private int originalIndex;

    [Header("ShakeEffect")]
    private bool isShaking = false;
    private float shakeTime = 0f;
    private float shakeDuration = 0.3f;
    private float shakeStrength = 10f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Update()
    {
        if (isDragging && !isShaking) return;

        Vector3 shakeOffset = Vector3.zero;

        if (isShaking)
        {
            shakeTime -= Time.deltaTime;

            float strength = (shakeTime / shakeDuration) * shakeStrength;

            float frequency = 40f;

            shakeOffset = new Vector3(
                Mathf.Sin(Time.time * frequency) * strength,
                0,
                0
            );

            if (shakeTime <= 0)
            {
                isShaking = false;
            }
        }

        Vector3 target = basePosition;

        if (isHovering)
            target += Vector3.up * hoverHeight;

        Vector3 smoothPos = Vector3.Lerp(
        rectTransform.localPosition,
        target,
        Time.deltaTime * 10f
        );
        rectTransform.localPosition = smoothPos + shakeOffset;


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

        originalParent = transform.parent;
        originalIndex = transform.GetSiblingIndex();
        
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
        //SE SAIU DA MÃO
        if (!hasLeftHand && !handManager.IsCardInHandZone(this))
        {
            hasLeftHand = true;
            transform.SetParent(canvas.transform);
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out mousePos
            );

            dragOffset = mousePos - (Vector2)rectTransform.localPosition;
        }
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
            transform.SetParent(originalParent);
            transform.SetSiblingIndex(originalIndex);
        }

        handManager.draggedCard = null;
        canvasGroup.blocksRaycasts = true;
        hasLeftHand = false;
    }
    #endregion

    public void TriggerShake()
    {
        isShaking = true;
        shakeTime = shakeDuration;
    }
}