using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    public HandManager handManager;
    private DeckManager deckManager;
    private PlayArea playArea;
    private WarningUI warningUI;
    private ManaManagerSTS manaManager;

    [Header("Validation Values")]
    private bool isDragging;
    private bool isHovering;
    private float rotationSpeed = 15f;
    private bool isReturning = false;
    private float returnSpeed = 15f;
    private bool isWaitingForTarget = false;

    [Header("Hover")]
    public float hoverScale = 1.2f;
    public float hoverHeight = 80f;
    public GameObject glow;

    [Header("Hover Colors")]
    public Color normalHoverColor = new Color(0f,0.9f,1f,0.6f);
    public Color noManaHoverColor = new Color(1f, 0.2f, 0.2f, 0.6f); // vermelho claro
    private Image glowImage;

    [Header("DragMovement")]
    private bool hasLeftHand = false;
    private Transform originalParent;
    private int originalIndex;
    private bool isInPlayArea = false;

    [Header("ShakeEffect")]
    private bool isShaking = false;
    private float shakeTime = 0f;
    private float shakeDuration = 0.3f;
    private float shakeStrength = 10f;

    [Header("CardPulsation")]
    private float pulseSpeed = 6f;
    private float pulseAmount = 0.03f;
    private bool isPulsing = false;
    private Vector3 baseScale;


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

        pulseSpeed = 6f;
        pulseAmount = .03f;

    }

    void Update()
    {
        if (isInPlayArea)
        {
            if (isPulsing)
            {
                float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
                transform.localScale = baseScale * pulse;
            }

            return;
        }
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
        if (handManager == null) return;
        if (isDragging || handManager.draggedCard != null || isWaitingForTarget || PlayArea.HasCardInPlay) return;

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
        if (handManager == null) return;
        if (isDragging || handManager.draggedCard != null || isWaitingForTarget || PlayArea.HasCardInPlay) return;

        isHovering = false;

        if (glow) glow.SetActive(false);

        if (handManager.hoveredCard == this)
            handManager.hoveredCard = null;

    }
    #endregion

    #region DRAG
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (PlayArea.HasCardInPlay && !isInPlayArea) return;

        isDragging = true;
        isHovering = false;

        if (glow != null)
            glow.SetActive(false);

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
            // estado global
            PlayArea.HasCardInPlay = true;

            #region POSIÇÃO NO PLAYAREA

            // 🔥 salva posição MUNDIAL antes de mudar parent
            Vector3 worldPos = rectTransform.position;

            // move pro playArea SEM mexer na posição visual
            RectTransform playRect = playArea.GetComponent<RectTransform>();
            transform.SetParent(playRect, true); // 🔥 TRUE mantém posição global

            // 🔥 agora converte posição para LOCAL do playArea
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                playRect,
                RectTransformUtility.WorldToScreenPoint(null, worldPos),
                null,
                out localPoint
            );

            // aplica posição LOCAL correta
            rectTransform.anchoredPosition = localPoint;

            // reset leve
            rectTransform.localScale = Vector3.one;
            rectTransform.localRotation = Quaternion.identity;

            // marca estado
            isInPlayArea = true;

            // 🔥 anima até o centro (ZERO)
            StartCoroutine(AnimateToPlayArea(Vector2.zero));

            // evita interferência do layout
            SetBasePosition(Vector3.zero);

            #endregion

            CardDisplay display = GetComponent<CardDisplay>();

            if (display == null || display.cardData == null || manaManager == null)
            {
                Debug.LogError("Erro: referências faltando ao jogar carta");
                ReturnToHandSafe();
                EndDragCleanup();
                return;
            }

            int cost = display.cardData.cardMana;

            // ❌ SEM MANA
            if (!manaManager.HasEnoughMana(cost))
            {
                ReturnToHandSafe();

                if (warningUI != null)
                    warningUI.Show("Not enough mana");

                handManager.ShakeHand();
                EndDragCleanup();
                return;
            }

            // 🔥 CARTA COM TARGET
            if (display.cardData.requiresTarget)
            {
                
                if (TargetManager.Instance == null)
                {
                    Debug.LogError("TargetManager não encontrado na cena!");
                    ReturnToHandSafe();
                    EndDragCleanup();
                    return;
                }
                //// 🔥 GARANTE atualização visual
                Canvas.ForceUpdateCanvases();
                StartPulse();
                TargetManager.Instance.StartTargeting(

                    // 🎯 AO SELECIONAR
                    (slot) =>
                    {
                        isWaitingForTarget = false;
                        PlayArea.HasCardInPlay = false;
                        manaManager.SpendMana(cost);
                        Debug.Log("CARD EFFECT: " + display.cardData.textFront);
                        handManager.owner.PlayCard(display.cardData, gameObject);
                        StopPulse();
                    },

                    // ❌ AO CANCELAR
                    () =>
                    {
                        Debug.Log("Cancelou a carta");

                        isWaitingForTarget = false;
                        PlayArea.HasCardInPlay = false;
                        isInPlayArea= false;
                        ReturnToHandSafe();
                        StopPulse();
                    }
                );

                EndDragCleanup();
                return;
            }
            else
            {
                // 🔥 SEM TARGET
                manaManager.SpendMana(cost);
                PlayArea.HasCardInPlay = false;
                Debug.Log("CARD EFFECT: " + display.cardData.textFront);
                handManager.owner.PlayCard(display.cardData, gameObject);
                EndDragCleanup();
                return;
            }
        }
        else
        {
            ReturnToHandSafe();
            EndDragCleanup();
        }
    }
    #endregion

    public void TriggerShake()
    {
        isShaking = true;
        shakeTime = shakeDuration;
    }

    void ReturnToHandSafe()
    {
        isInPlayArea = false;
        transform.SetParent(originalParent, true);
        transform.SetSiblingIndex(originalIndex);

        isHovering = false;

        if (glow != null)
            glow.SetActive(false);

        handManager.hoveredCard = null;

        transform.localScale = Vector3.one;
        PlayArea.HasCardInPlay = false;
        isWaitingForTarget = false;
        canvasGroup.interactable = true;
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

    IEnumerator AnimateToPlayArea(Vector2 target)
    {
        Vector2 start = rectTransform.anchoredPosition;

        float duration = 0.2f;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / duration;

            float curve = Mathf.SmoothStep(0, 1, t);

            rectTransform.anchoredPosition = Vector2.Lerp(start, target, curve);

            yield return null;
        }

        rectTransform.anchoredPosition = target;
    }

    void StartPulse()
    {
        isPulsing = true;
        baseScale = Vector3.one;
    }

    void StopPulse()
    {
        isPulsing = false;
        transform.localScale = Vector3.one;
    }
}