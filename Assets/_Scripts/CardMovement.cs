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

    [Header("GameObjects/Scripts")]
    private HandManager handManager;
    private DeckManager deckManager;
    private PlayArea playArea;
    private WarningUI warningUI;
    private ManaManagerSTS manaManager;

    private bool isDragging;
    private bool isHovering;
    private float rotationSpeed = 15f;
    private bool isReturning = false;
    private float returnSpeed = 15f;

    [Header("Hover")]
    public float hoverScale = 1.2f;
    public float hoverHeight = 80f;
    public GameObject glow;

    [Header("Hover Colors")]
    public Color normalHoverColor = new Color(0f,0.9f,1f,0.6f);
    public Color noManaHoverColor = new Color(1f, 0.2f, 0.2f, 0.6f); // vermelho claro
    private UnityEngine.UI.Image glowImage;

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

        if (glow != null)
            glow.SetActive(false);
        if (glow != null)
            glowImage = glow.GetComponent<UnityEngine.UI.Image>();

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

        if (isReturning)
        {
            rectTransform.localPosition = Vector3.Lerp(
                rectTransform.localPosition,
                basePosition,
                Time.deltaTime * returnSpeed
            );

            if (Vector3.Distance(rectTransform.localPosition, basePosition) < 1f)
            {
                rectTransform.localPosition = basePosition;
                isReturning = false;
            }

            return; 
        }

        // 🔥 HOVER / IDLE
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

        if (isDragging)
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.identity,
                Time.deltaTime * rotationSpeed
            );
        }
    }

    public void SetHandManager(HandManager manager)
    {
        handManager = manager;
        canvas = manager.GetComponentInParent<Canvas>();
        deckManager = FindAnyObjectByType<DeckManager>();
        playArea = PlayArea.Instance;
        manaManager = FindAnyObjectByType<ManaManagerSTS>();
        warningUI = FindAnyObjectByType<WarningUI>();
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

        if (glowImage != null)
        {
            glowImage.color = HasEnoughMana() ? normalHoverColor : noManaHoverColor;
        }

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

        dragOffset = Vector2.zero;

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;

        //SE SAIU DA MÃO
        if (!hasLeftHand && !handManager.IsCardInHandZone(this))
        {
            hasLeftHand = true;
            transform.SetParent(canvas.transform, true);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        bool played = false;

        if (playArea != null)
            played = playArea.IsInside(rectTransform);

        if (played)
        {
            CardDisplay display = GetComponent<CardDisplay>();

            if (display == null || display.cardData == null || manaManager == null)
            {
                Debug.LogError("Erro: referências faltando ao jogar carta");
                ReturnToHandSafe();
                EndDragCleanup();
                return;
            }

            int cost = display.cardData.cardMana;
            if (!manaManager.HasEnoughMana(cost))
            {
                ReturnToHandSafe();
                if (warningUI != null)
                    warningUI.Show("Not enough mana");
                handManager.ShakeHand();
                EndDragCleanup();
                return;
            }

            // ✅ JOGOU COM SUCESSO
            manaManager.SpendMana(cost);

            if (deckManager != null)
                deckManager.DiscardCard(gameObject);
        }
        else
        {
            // ❌ NÃO JOGOU
            ReturnToHandSafe();
        }
        EndDragCleanup();
    }
    #endregion

    public void TriggerShake()
    {
        isShaking = true;
        shakeTime = shakeDuration;
    }

    void ReturnToHandSafe()
    {
        transform.SetParent(originalParent, true);
        transform.SetSiblingIndex(originalIndex);

        isHovering = false;

        if (glow != null)
            glow.SetActive(false);

        handManager.hoveredCard = null;

        transform.localScale = Vector3.one;

        //transform.localRotation = Quaternion.identity;
        
        isReturning = true;

        canvasGroup.blocksRaycasts = true;
        handManager.UpdateCardsList();
        
    }

    void EndDragCleanup()
    {
        handManager.draggedCard = null;
        handManager.hoveredCard = null;

        canvasGroup.blocksRaycasts = true;

        isDragging = false;
        hasLeftHand = false;
    }

    public bool IsReturning()
    {
        return isReturning;
    }

    bool HasEnoughMana()
    {
        if (manaManager == null) return true;

        CardDisplay display = GetComponent<CardDisplay>();
        if (display == null || display.cardData == null) return true;

        return manaManager.HasEnoughMana(display.cardData.cardMana);
    }
}