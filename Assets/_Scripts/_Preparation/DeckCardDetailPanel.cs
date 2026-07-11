using CardData;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Controla o painel de detalhes de uma carta no DeckBuilder.
///
/// Setup no Inspector:
///   - panelRoot:       GameObject raiz do painel (com CanvasGroup)
///   - cardDisplayRoot: Transform onde o CardDetailPrefab será instanciado
///   - cardDetailPrefab: prefab com CardDetailDisplay.cs
/// </summary>
public class DeckCardDetailPanel : MonoBehaviour, IPointerClickHandler
{
    [Header("Referências")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private RectTransform cardDisplayRoot;
    [SerializeField] private GameObject cardDetailPrefab;

    private CanvasGroup canvasGroup;
    private CardDetailDisplay currentDisplay;

    void Awake()
    {
        canvasGroup = panelRoot != null
            ? panelRoot.GetComponent<CanvasGroup>()
            : GetComponent<CanvasGroup>();

        if (canvasGroup == null && panelRoot != null)
            canvasGroup = panelRoot.AddComponent<CanvasGroup>();

        SetVisible(false);
    }

    public void Open(Card card)
    {

        if (card == null) return; 

        ClearCurrentCard();

        if (cardDetailPrefab != null && cardDisplayRoot != null)
        {
            GameObject instance = Instantiate(cardDetailPrefab, cardDisplayRoot);

            RectTransform rt = instance.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = Vector2.zero;
                rt.localScale = Vector3.one;
            }

            CardDetailDisplay display = instance.GetComponent<CardDetailDisplay>();
            if (display != null)
            {
                display.Populate(card);
                currentDisplay = display;
            }
        }

        SetVisible(true);
    }

    public void Close()
    {
        ClearCurrentCard();
        SetVisible(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Fecha apenas se o clique foi no backdrop (este GameObject ou o Backdrop filho),
        // não dentro da carta instanciada.
        if (currentDisplay != null &&
            eventData.pointerCurrentRaycast.gameObject != null &&
            eventData.pointerCurrentRaycast.gameObject.transform.IsChildOf(currentDisplay.transform))
        {
            return;
        }

        Close();
    }

    void ClearCurrentCard()
    {
        if (currentDisplay != null)
        {
            Destroy(currentDisplay.gameObject);
            currentDisplay = null;
        }
    }

    void SetVisible(bool visible)
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }
}