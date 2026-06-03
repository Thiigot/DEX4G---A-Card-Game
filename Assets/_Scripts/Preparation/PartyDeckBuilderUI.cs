using System.Collections.Generic;
using CardData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyDeckBuilderUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text deckCountText;
    [SerializeField] private Transform availableContent;
    [SerializeField] private Transform deckContent;
    [SerializeField] private DeckCardListItem listItemPrefab;
    [SerializeField] private Button addButton;
    [SerializeField] private Button removeButton;
    [SerializeField] private Button closeButton;

    private UnitData currentUnit;
    private Card selectedAvailableCard;
    private Card selectedDeckCard;
    private readonly List<DeckCardListItem> spawnedItems = new();
    private GameObject panelRoot;
    private CanvasGroup panelCanvasGroup;

    void Awake()
    {
        panelRoot = panel != null ? panel : gameObject;
        panelCanvasGroup = panelRoot.GetComponent<CanvasGroup>();
        if (panelCanvasGroup == null)
            panelCanvasGroup = panelRoot.AddComponent<CanvasGroup>();

        SetPanelVisible(false);

        if (addButton != null)
            addButton.onClick.AddListener(AddSelectedCard);

        if (removeButton != null)
            removeButton.onClick.AddListener(RemoveSelectedCard);

        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
    }

    public void Open(UnitData unit)
    {
        currentUnit = unit;
        selectedAvailableCard = null;
        selectedDeckCard = null;

        SetPanelVisible(true);

        Rebuild();
    }

    public void Close()
    {
        currentUnit = null;

        SetPanelVisible(false);

        ClearItems();
    }

    void AddSelectedCard()
    {
        if (currentUnit == null || selectedAvailableCard == null) return;

        PartyDeckState.GetDeck(currentUnit).Add(selectedAvailableCard);
        Rebuild();
    }

    void RemoveSelectedCard()
    {
        if (currentUnit == null || selectedDeckCard == null) return;

        PartyDeckState.GetDeck(currentUnit).Remove(selectedDeckCard);
        selectedDeckCard = null;
        Rebuild();
    }

    void Rebuild()
    {
        ClearItems();

        if (currentUnit == null || listItemPrefab == null)
            return;

        if (titleText != null)
            titleText.text = $"{currentUnit.unitName} Deck";

        List<Card> deck = PartyDeckState.GetDeck(currentUnit);

        if (deckCountText != null)
            deckCountText.text = $"Deck: {deck.Count}";

        foreach (Card card in Resources.LoadAll<Card>(currentUnit.unitName))
            SpawnItem(availableContent, card, card == selectedAvailableCard, SelectAvailableCard);

        foreach (Card card in deck)
            SpawnItem(deckContent, card, card == selectedDeckCard, SelectDeckCard);
    }

    void SpawnItem(Transform parent, Card card, bool selected, System.Action<Card> onClick)
    {
        if (parent == null) return;

        DeckCardListItem item = Instantiate(listItemPrefab, parent);
        item.Setup(card, selected, onClick);
        spawnedItems.Add(item);
    }

    void SelectAvailableCard(Card card)
    {
        selectedAvailableCard = card;
        Rebuild();
    }

    void SelectDeckCard(Card card)
    {
        selectedDeckCard = card;
        Rebuild();
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

    void SetPanelVisible(bool visible)
    {
        if (panelCanvasGroup == null)
            return;

        panelCanvasGroup.alpha = visible ? 1f : 0f;
        panelCanvasGroup.interactable = visible;
        panelCanvasGroup.blocksRaycasts = visible;
    }
}
