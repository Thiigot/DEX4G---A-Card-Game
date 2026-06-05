using System.Collections.Generic;
using CardData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyDeckBuilderUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text deckCountText;
    [SerializeField] private int panelSortingOrder = 300;

    [Header("Displays")]
    [SerializeField] private RectTransform collectionDropArea;
    [SerializeField] private RectTransform deckDropArea;
    [SerializeField] private ScrollRect collectionScrollRect;
    [SerializeField] private ScrollRect deckScrollRect;
    [SerializeField] private Transform collectionContent;
    [SerializeField] private Transform deckContent;
    [SerializeField] private DeckCardListItem listItemPrefab;

    [Header("Card Details")]
    [SerializeField] private TMP_Text detailNameText;
    [SerializeField] private TMP_Text detailCostText;
    [SerializeField] private TMP_Text detailTypeText;
    [SerializeField] private TMP_Text detailFrontText;
    [SerializeField] private TMP_Text detailBackText;

    [Header("Actions")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button exitButton;

    public Transform DragRoot => dragRoot != null ? dragRoot : transform;

    private UnitData currentUnit;
    private Card selectedCard;
    private readonly List<DeckCardListItem> spawnedItems = new();
    private GameObject panelRoot;
    private CanvasGroup panelCanvasGroup;
    private Canvas panelCanvas;
    private Transform dragRoot;

    void Awake()
    {
        panelRoot = panel != null ? panel : gameObject;
        panelCanvasGroup = panelRoot.GetComponent<CanvasGroup>();
        if (panelCanvasGroup == null)
            panelCanvasGroup = panelRoot.AddComponent<CanvasGroup>();

        panelCanvas = panelRoot.GetComponent<Canvas>();
        if (panelCanvas == null)
            panelCanvas = panelRoot.AddComponent<Canvas>();

        panelCanvas.overrideSorting = true;
        panelCanvas.sortingOrder = panelSortingOrder;

        if (panelRoot.GetComponent<GraphicRaycaster>() == null)
            panelRoot.AddComponent<GraphicRaycaster>();

        dragRoot = panelRoot.transform;

        SetPanelVisible(false);

        if (saveButton != null)
            saveButton.onClick.AddListener(SaveChanges);

        if (exitButton != null)
            exitButton.onClick.AddListener(ExitWithoutSaving);
    }

    public void Open(UnitData unit)
    {
        if (unit == null)
            return;

        currentUnit = unit;
        selectedCard = null;

        PartyDeckState.BeginDraft(currentUnit);
        LogDeckBuilderState();

        SetPanelVisible(true);
        panelRoot.transform.SetAsLastSibling();

        Rebuild();
        ClearCardDetails();
    }

    public void Close()
    {
        currentUnit = null;
        selectedCard = null;

        SetPanelVisible(false);
        ClearItems();
        ClearCardDetails();
    }

    public void ShowCardDetails(Card card)
    {
        selectedCard = card;

        if (card == null)
        {
            ClearCardDetails();
            return;
        }

        if (detailNameText != null)
            detailNameText.text = card.cardName;

        if (detailCostText != null)
            detailCostText.text = $"Cost: {card.cardMana}";

        if (detailTypeText != null)
            detailTypeText.text = card.cardType.ToString();

        if (detailFrontText != null)
            detailFrontText.text = card.textInFront;

        if (detailBackText != null)
            detailBackText.text = card.textInBack;

        Rebuild();
    }

    public void QuickMoveCard(Card card, DeckBuilderListType sourceList)
    {
        DeckBuilderListType destination = sourceList == DeckBuilderListType.Collection
            ? DeckBuilderListType.Deck
            : DeckBuilderListType.Collection;

        MoveCard(card, sourceList, destination);
    }

    public void MoveCard(Card card, DeckBuilderListType sourceList, DeckBuilderListType destination)
    {
        if (currentUnit == null || card == null || sourceList == destination)
            return;

        if (destination == DeckBuilderListType.Deck)
        {
            bool added = PartyDeckState.TryAddToDraft(currentUnit, card);
            Debug.Log(added
                ? $"DeckBuilder: added {card.cardName} to {currentUnit.unitName} deck."
                : $"DeckBuilder: could not add {card.cardName}. Deck may be full or card is already in deck.");
        }

        if (destination == DeckBuilderListType.Collection)
        {
            bool removed = PartyDeckState.RemoveFromDraft(currentUnit, card);
            Debug.Log(removed
                ? $"DeckBuilder: removed {card.cardName} from {currentUnit.unitName} deck."
                : $"DeckBuilder: could not remove {card.cardName}. It was not in deck.");
        }

        Rebuild();
    }

    public DeckBuilderListType? ResolveDropTarget(Vector2 screenPosition)
    {
        Camera eventCamera = GetEventCamera();

        if (collectionDropArea != null &&
            RectTransformUtility.RectangleContainsScreenPoint(collectionDropArea, screenPosition, eventCamera))
            return DeckBuilderListType.Collection;

        if (deckDropArea != null &&
            RectTransformUtility.RectangleContainsScreenPoint(deckDropArea, screenPosition, eventCamera))
            return DeckBuilderListType.Deck;

        return null;
    }

    Camera GetEventCamera()
    {
        if (panelCanvas == null || panelCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return null;

        return panelCanvas.worldCamera != null ? panelCanvas.worldCamera : Camera.main;
    }

    void SaveChanges()
    {
        if (currentUnit == null)
            return;

        PartyDeckState.SaveDraft(currentUnit);
        Rebuild();
    }

    void ExitWithoutSaving()
    {
        if (currentUnit != null)
            PartyDeckState.DiscardDraft(currentUnit);

        Close();
    }

    void Rebuild()
    {
        ClearItems();

        if (currentUnit == null || listItemPrefab == null)
        {
            if (currentUnit == null)
                Debug.LogWarning("DeckBuilder: currentUnit está NULL.");

            if (listItemPrefab == null)
                Debug.LogWarning("DeckBuilder: List Item Prefab não está conectado no Inspector.");

            return;
        }

        if (collectionContent == null)
            Debug.LogWarning("DeckBuilder: Collection Content não está conectado no Inspector.");

        if (deckContent == null)
            Debug.LogWarning("DeckBuilder: Deck Content não está conectado no Inspector.");

        List<Card> draftDeck = PartyDeckState.GetDraft(currentUnit);
        IReadOnlyList<Card> availableCards = PartyDeckState.GetAvailableCards(currentUnit);

        Debug.Log(
            $"DeckBuilder Rebuild: unit={currentUnit.unitName}, " +
            $"availableCards={availableCards.Count}, draftDeck={draftDeck.Count}, " +
            $"collectionContent={(collectionContent != null ? collectionContent.name : "NULL")}, " +
            $"deckContent={(deckContent != null ? deckContent.name : "NULL")}, " +
            $"listItemPrefab={(listItemPrefab != null ? listItemPrefab.name : "NULL")}"
        );

        if (titleText != null)
            titleText.text = $"{currentUnit.unitName} Deck Builder";

        if (deckCountText != null)
            deckCountText.text = $"Deck: {draftDeck.Count}/{PartyDeckState.MaxDeckSize}";

        foreach (Card card in availableCards)
        {
            if (card == null) continue;

            bool selected = card == selectedCard;
            bool alreadyInDeck = draftDeck.Contains(card);
            SpawnItem(collectionContent, card, DeckBuilderListType.Collection, selected, alreadyInDeck);
        }

        foreach (Card card in draftDeck)
        {
            if (card == null) continue;
            SpawnItem(deckContent, card, DeckBuilderListType.Deck, card == selectedCard, false);
        }

        Canvas.ForceUpdateCanvases();

        if (collectionScrollRect != null)
            collectionScrollRect.verticalNormalizedPosition = 1f;

        if (deckScrollRect != null)
            deckScrollRect.verticalNormalizedPosition = 1f;
    }

    void SpawnItem(Transform parent, Card card, DeckBuilderListType sourceList, bool selected, bool unavailable)
    {
        if (parent == null)
        {
            Debug.LogWarning($"DeckBuilder: nao foi possivel criar item de {card.cardName}, parent esta NULL.");
            return;
        }

        DeckCardListItem item = Instantiate(listItemPrefab, parent);
        item.Setup(card, this, sourceList, selected, unavailable);
        spawnedItems.Add(item);
    }

    void ClearItems()
    {
        foreach (DeckCardListItem item in spawnedItems)
        {
            if (item != null)
                Destroy(item.gameObject);
        }

        spawnedItems.Clear();
    }

    void ClearCardDetails()
    {
        if (detailNameText != null)
            detailNameText.text = "";

        if (detailCostText != null)
            detailCostText.text = "";

        if (detailTypeText != null)
            detailTypeText.text = "";

        if (detailFrontText != null)
            detailFrontText.text = "";

        if (detailBackText != null)
            detailBackText.text = "";
    }

    void SetPanelVisible(bool visible)
    {
        if (panelCanvasGroup == null)
            return;

        panelCanvasGroup.alpha = visible ? 1f : 0f;
        panelCanvasGroup.interactable = visible;
        panelCanvasGroup.blocksRaycasts = visible;
    }

    void LogDeckBuilderState()
    {
        if (currentUnit == null)
            return;

        if (currentUnit.deckData == null)
        {
            Debug.LogWarning($"DeckBuilder: {currentUnit.unitName} não possui DeckData conectado no UnitData.");
            return;
        }

        int availableCount = currentUnit.deckData.availableCards != null
            ? currentUnit.deckData.availableCards.Count
            : 0;

        int savedCount = currentUnit.deckData.startingDeck != null
            ? currentUnit.deckData.startingDeck.Count
            : 0;

        Debug.Log(
            $"DeckBuilder aberto para {currentUnit.unitName}. " +
            $"DeckData: {currentUnit.deckData.name}. " +
            $"AvailableCards: {availableCount}. StartingDeck: {savedCount}."
        );

        if (availableCount == 0)
            Debug.LogWarning($"DeckBuilder: {currentUnit.deckData.name} está com Available Cards vazio.");
    }
}
