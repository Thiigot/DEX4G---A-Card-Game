using System.Collections;
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

    [Header("Card Detail Panel")]
    [SerializeField] private DeckCardDetailPanel cardDetailPanel;

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

        // Reseta scroll para o topo apenas na abertura inicial do painel.
        if (collectionScrollRect != null)
            collectionScrollRect.verticalNormalizedPosition = 1f;

        if (deckScrollRect != null)
            deckScrollRect.verticalNormalizedPosition = 1f;
    }

    public void Close()
    {
        currentUnit = null;
        selectedCard = null;

        SetPanelVisible(false);
        ClearItems();
        ClearCardDetails();
    }

    /// <summary>
    /// Abre o painel visual de detalhes da carta (carta em tamanho grande).
    /// Chamado pelo clique direito no DeckCardListItem.
    /// </summary>
    public void OpenCardDetail(Card card)
    {
        if (cardDetailPanel != null)
            cardDetailPanel.Open(card);
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

    public bool MoveCard(Card card, DeckBuilderListType sourceList, DeckBuilderListType destination)
    {
        if (currentUnit == null || card == null || sourceList == destination)
            return false;

        bool changed = false;

        if (destination == DeckBuilderListType.Deck)
        {
            changed = PartyDeckState.TryAddToDraft(currentUnit, card);
        }

        if (destination == DeckBuilderListType.Collection)
        {
            changed = PartyDeckState.RemoveFromDraft(currentUnit, card);
        }

        if (changed)
            Rebuild();

        return changed;
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
        // Salva posição atual dos scrolls antes de destruir e recriar os itens.
        // Só reseta para o topo na primeira abertura (Open), nunca durante edição.
        float collectionScroll = collectionScrollRect != null ? collectionScrollRect.verticalNormalizedPosition : 1f;
        float deckScroll = deckScrollRect != null ? deckScrollRect.verticalNormalizedPosition : 1f;

        if (restoreScrollCoroutine != null)
            StopCoroutine(restoreScrollCoroutine);

        ClearItems();

        if (currentUnit == null || listItemPrefab == null)
        {
            return;
        }

        List<Card> draftDeck = PartyDeckState.GetDraft(currentUnit);
        IReadOnlyList<Card> availableCards = PartyDeckState.GetAvailableCards(currentUnit);

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

        // Restaura a posição no frame seguinte, após o layout estar estabilizado.
        // Restaurar imediatamente causaria um "piscar" visível porque o Content
        // acaba de ser recriado e o Unity ainda não calculou as alturas corretas.
        restoreScrollCoroutine = StartCoroutine(RestoreScrollNextFrame(collectionScroll, deckScroll));
    }

    private Coroutine restoreScrollCoroutine;

    IEnumerator RestoreScrollNextFrame(float collectionPos, float deckPos)
    {
        // WaitForEndOfFrame garante que o VerticalLayoutGroup e o ContentSizeFitter
        // já terminaram de recalcular as alturas dos itens recém-criados antes de
        // restaurar a posição — evita o "piscar" causado por restaurar cedo demais.
        yield return new WaitForEndOfFrame();

        LayoutRebuilder.ForceRebuildLayoutImmediate(collectionContent as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(deckContent as RectTransform);

        Canvas.ForceUpdateCanvases();

        if (collectionScrollRect != null)
            collectionScrollRect.verticalNormalizedPosition = collectionPos;

        if (deckScrollRect != null)
            deckScrollRect.verticalNormalizedPosition = deckPos;
    }

    void SpawnItem(Transform parent, Card card, DeckBuilderListType sourceList, bool selected, bool unavailable)
    {
        if (parent == null)
        {
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
            return;
        }

        int availableCount = currentUnit.deckData.availableCards != null
            ? currentUnit.deckData.availableCards.Count
            : 0;

        int savedCount = currentUnit.deckData.startingDeck != null
            ? currentUnit.deckData.startingDeck.Count
            : 0;
    }
}
