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
    [SerializeField] private TMP_Text typeLabel;
    [SerializeField] private Image background;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(0.45f, 0.8f, 1f, 1f);
    [SerializeField] private Color unavailableColor = new Color(0.45f, 0.45f, 0.45f, 0.75f);

    private Card card;
    private PartyDeckBuilderUI owner;
    private DeckBuilderListType listType;
    private RectTransform rectTransform;
    private Transform originalParent;
    private int originalSiblingIndex;

    void Awake()
    {
        rectTransform = transform as RectTransform;

        if (background == null)
            background = GetComponent<Image>();

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

        if (costLabel != null)
            costLabel.text = card != null ? card.cardMana.ToString() : "";

        if (typeLabel != null)
            typeLabel.text = card != null ? card.cardType.ToString() : "";

        if (background != null)
        {
            if (unavailable)
                background.color = unavailableColor;
            else
                background.color = selected ? selectedColor : normalColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (owner == null || card == null)
            return;

        Debug.Log($"DeckBuilder card click: {card.cardName}, button={eventData.button}, source={listType}");

        if (eventData.button == PointerEventData.InputButton.Left)
            owner.ShowCardDetails(card);

        if (eventData.button == PointerEventData.InputButton.Right)
            owner.QuickMoveCard(card, listType);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (owner == null || card == null || rectTransform == null)
            return;

        Debug.Log($"DeckBuilder drag begin: {card.cardName}, source={listType}");

        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();

        transform.SetParent(owner.DragRoot, true);
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.85f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rectTransform == null)
            return;

        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        DeckBuilderListType? destination = owner != null
            ? owner.ResolveDropTarget(eventData.position)
            : null;

        Debug.Log(
            $"DeckBuilder drag end: {card.cardName}, source={listType}, " +
            $"destination={(destination.HasValue ? destination.Value.ToString() : "None")}"
        );

        if (owner != null && destination.HasValue)
            owner.MoveCard(card, listType, destination.Value);

        RestorePosition();
    }

    void RestorePosition()
    {
        if (originalParent == null)
            return;

        transform.SetParent(originalParent, false);
        transform.SetSiblingIndex(originalSiblingIndex);
    }
}
