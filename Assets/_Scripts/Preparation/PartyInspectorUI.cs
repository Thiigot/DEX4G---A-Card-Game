using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyInspectorUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text manaText;
    [SerializeField] private Button backButton;
    [SerializeField] private Button editDeckButton;
    [SerializeField] private PartyDeckBuilderUI deckBuilderUI;
    [SerializeField] private Image panelBackground;
    [SerializeField] private Color activePanelColor = new Color(0.08f, 0.08f, 0.1f, 0.96f);
    [SerializeField] private bool forcePanelCanvasOnTop = true;
    [SerializeField] private int panelSortingOrder = 200;

    private UnitData selectedUnit;
    private bool subscribed;
    private GameObject panelRoot;
    private CanvasGroup panelCanvasGroup;
    private Canvas panelCanvas;

    void Awake()
    {
        panelRoot = panel != null ? panel : gameObject;
        panelCanvasGroup = panelRoot.GetComponent<CanvasGroup>();
        if (panelCanvasGroup == null)
            panelCanvasGroup = panelRoot.AddComponent<CanvasGroup>();

        if (panelBackground == null)
            panelBackground = panelRoot.GetComponent<Image>();

        EnsurePanelRendersOnTop();
        SetPanelVisible(false);

        if (backButton != null)
            backButton.onClick.AddListener(CloseDetails);

        if (editDeckButton != null)
            editDeckButton.onClick.AddListener(EditSelectedDeck);
    }

    void OnEnable()
    {
        Subscribe();
    }

    void Start()
    {
        Subscribe();
    }

    void OnDisable()
    {
        if (PartyManager.Instance != null && subscribed)
            PartyManager.Instance.OnDetailsUnitChanged -= Show;

        subscribed = false;
    }

    public void Show(UnitData unit)
    {
        selectedUnit = unit;

        SetPanelVisible(unit != null);
        BringPanelToFront();

        if (unit == null)
            return;

        if (panelBackground != null)
            panelBackground.color = activePanelColor;

        if (nameText != null)
            nameText.text = unit.unitName;

        if (hpText != null)
            hpText.text = $"HP: {unit.maxHP}";

        if (attackText != null)
            attackText.text = $"ATK: {unit.attack}";

        if (speedText != null)
            speedText.text = $"SPD: {unit.speed}";

        if (manaText != null)
            manaText.text = $"MANA: {unit.baseMana}";
    }

    void CloseDetails()
    {
        if (PartyManager.Instance == null)
            return;

        PartyManager.Instance.CloseDetails();
    }

    void EditSelectedDeck()
    {
        if (selectedUnit == null || deckBuilderUI == null)
            return;

        deckBuilderUI.Open(selectedUnit);
    }

    void Subscribe()
    {
        if (subscribed || PartyManager.Instance == null)
            return;

        PartyManager.Instance.OnDetailsUnitChanged += Show;
        subscribed = true;
    }

    void SetPanelVisible(bool visible)
    {
        if (panelCanvasGroup == null)
            return;

        panelCanvasGroup.alpha = visible ? 1f : 0f;
        panelCanvasGroup.interactable = visible;
        panelCanvasGroup.blocksRaycasts = visible;
    }

    void EnsurePanelRendersOnTop()
    {
        if (!forcePanelCanvasOnTop || panelRoot == null)
            return;

        panelCanvas = panelRoot.GetComponent<Canvas>();
        if (panelCanvas == null)
            panelCanvas = panelRoot.AddComponent<Canvas>();

        panelCanvas.overrideSorting = true;
        panelCanvas.sortingOrder = panelSortingOrder;

        if (panelRoot.GetComponent<GraphicRaycaster>() == null)
            panelRoot.AddComponent<GraphicRaycaster>();
    }

    void BringPanelToFront()
    {
        if (panelRoot == null)
            return;

        panelRoot.transform.SetAsLastSibling();

        if (panelCanvas != null)
            panelCanvas.sortingOrder = panelSortingOrder;
    }
}
