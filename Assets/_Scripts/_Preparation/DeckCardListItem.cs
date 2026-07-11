using CardData;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum DeckBuilderListType
{
    Collection,
    Deck
}

public class DeckCardListItem : MonoBehaviour,
    IPointerClickHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [SerializeField] private TMP_Text nameLabel;
    [SerializeField] private TMP_Text costLabel;
    [SerializeField] private Image cardArt;
    [SerializeField] private Image highlight;
    [SerializeField] private Image nameBlank;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Color selectedColor = new Color(0.45f, 0.8f, 1f, 1f);
    [SerializeField] private Color unavailableColor = new Color(0.45f, 0.45f, 0.45f, 0.75f);
    [SerializeField] private Image[] typeImages;

    [SerializeField]
    private Color32[] colorType =
    {
    new Color32(212, 184, 40, 255),   // jackpot
    new Color32(116, 40, 40, 255),    // outlaw
    new Color32(40, 40, 116, 255),    // captain
    new Color32(40, 116, 40, 255),    // wanderer
    new Color32(99, 99, 99, 255),     // mechanic
    new Color32(203, 203, 203, 255),  // jumper
    };
    private Card card;
    private PartyDeckBuilderUI owner;
    private DeckBuilderListType listType;
    private RectTransform rectTransform;
    private Transform originalParent;
    private int originalSiblingIndex;
    private DeckCardListItem placeholderItem;
    private bool isPlaceholder;



    void Awake()
    {
        rectTransform = transform as RectTransform;

        if (cardArt == null)
            cardArt = GetComponent<Image>();

        if (nameLabel == null)
            nameLabel = GetComponentInChildren<TMP_Text>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Setup(Card cardData, PartyDeckBuilderUI builder, DeckBuilderListType sourceList, bool selected, bool unavailable)
    {
        card = cardData;
        owner = builder;
        listType = sourceList;

        if (nameLabel != null)
            nameLabel.text = card != null ? card.cardName : "Empty";

        if(card.cardClass == CardClass.Mechanic || card.cardClass == CardClass.Jumper)
        {
            nameLabel.color = Color.black;
        }
        else
        {
            nameLabel.color = Color.white;
        }
        if (costLabel != null)
            costLabel.text = card != null ? card.cardMana.ToString() : "";

        
        if (cardArt != null && card != null)
        {
            cardArt.sprite = card.cardArt;
            Color c = new Color(1f,1f,1f,1f);
            if (unavailable)
                c = unavailableColor;
            cardArt.color = c;
        }

        if (highlight != null && card != null)
        {
            Color c = colorType[(int)card.cardClass];
            if (selected)
            {
                c = selectedColor;
                //highlight.gameObject.SetActive(true);
            }
            else if (unavailable)
            {
                c = unavailableColor;
            }
            else
            {
                //highlight.gameObject.SetActive(false);
            }

            highlight.color = c;
        }
        if (nameBlank != null && card != null)
        {
            Color c = colorType[(int)card.cardClass];
            if (unavailable)
                c = unavailableColor;
            nameBlank.color = c;
        }

        if(typeImages != null && card != null)
        {
            for (int i = 0; i < typeImages.Length; i++)
            {
                typeImages[i].gameObject.SetActive(i == (int)card.cardType);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isPlaceholder)
            return;

        if (owner == null || card == null)
            return;

        if (eventData.button == PointerEventData.InputButton.Right)
            owner.OpenCardDetail(card);

        if (eventData.button == PointerEventData.InputButton.Left)
            owner.QuickMoveCard(card, listType);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isPlaceholder)
            return;
        if (owner == null || card == null || rectTransform == null)
            return;

        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();

        CreateDragPlaceholder();

        transform.SetParent(owner.DragRoot, true);
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.85f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isPlaceholder)
            return;
        if (rectTransform == null || owner == null)
            return;
        RectTransform dragRootRect = owner.DragRoot as RectTransform;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dragRootRect,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );
        rectTransform.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isPlaceholder)
            return;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        transform.localScale = Vector3.one;
        DeckBuilderListType? destination = owner != null
            ? owner.ResolveDropTarget(eventData.position)
            : null;
        bool moved = false;
        if (owner != null && destination.HasValue)
            moved = owner.MoveCard(card, listType, destination.Value);

        ClearDragPlaceholder();
        if (!moved)
            RestorePosition();
    }

    void RestorePosition()
    {
        if (originalParent == null)
            return;

        transform.SetParent(originalParent, false);
        transform.SetSiblingIndex(originalSiblingIndex);
        transform.localScale = Vector3.one;
    }

    void CreateDragPlaceholder()
    {
        if (originalParent == null)
            return;

        DeckCardListItem placeholder = Instantiate(this, originalParent);
        placeholder.transform.SetSiblingIndex(originalSiblingIndex);
        placeholder.MakePlaceholder();
        placeholderItem = placeholder;
    }

    void MakePlaceholder()
    {
        isPlaceholder = true;

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.45f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        if (cardArt != null)
            cardArt.color = unavailableColor;
    }

    void ClearDragPlaceholder()
    {
        if (placeholderItem != null)
            Destroy(placeholderItem.gameObject);

        placeholderItem = null;
    }
}
