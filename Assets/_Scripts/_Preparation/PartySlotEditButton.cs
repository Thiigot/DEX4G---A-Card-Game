using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartySlotEditButton : MonoBehaviour
{
    private enum PreparationAction
    {
        Edit,
        Remove
    }

    [SerializeField] private PreparationAction action = PreparationAction.Edit;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;
    [SerializeField] private string disabledLabel = "";
    [SerializeField] private string occupiedLabel = "";
    [SerializeField] private float disabledAlpha = 0.35f;

    private GameObject visibleRoot;
    private CanvasGroup canvasGroup;
    private bool subscribed;

    void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        visibleRoot = gameObject;
        canvasGroup = visibleRoot.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = visibleRoot.AddComponent<CanvasGroup>();

        if (button != null)
            button.onClick.AddListener(ExecuteAction);
    }

    void OnEnable()
    {
        Subscribe();
        Refresh();
    }

    void Start()
    {
        Subscribe();
        Refresh();
    }

    void OnDisable()
    {
        if (PartyManager.Instance != null && subscribed)
        {
            PartyManager.Instance.OnSelectedUnitChanged -= HandleSelectedUnitChanged;
            PartyManager.Instance.OnDetailsUnitChanged -= HandleSelectedUnitChanged;
        }

        subscribed = false;
    }

    void ExecuteAction()
    {
        if (PartyManager.Instance == null)
            return;

        UnitData selectedUnit = PartyManager.Instance.SelectedUnit;
        if (selectedUnit == null)
            return;

        switch (action)
        {
            case PreparationAction.Edit:
                PartyManager.Instance.OpenSelectedDetails();
                PartyManager.Instance.Select(null);
                break;

            case PreparationAction.Remove:
                PartyManager.Instance.Remove(selectedUnit);
                PartyManager.Instance.Select(null);
                break;
        }
    }

    void Refresh()
    {
        if (PartyManager.Instance == null)
        {
            SetEnabled(false);
            return;
        }

        UnitData selectedUnit = PartyManager.Instance.SelectedUnit;
        bool hasSelection = selectedUnit != null && PartyManager.Instance.IsInParty(selectedUnit);
        bool detailsOpen = PartyManager.Instance.DetailsUnit != null;

        SetEnabled(hasSelection && !detailsOpen);

        if (label != null)
            label.text = hasSelection ? GetOccupiedLabel() : disabledLabel;
    }

    void SetEnabled(bool enabled)
    {
        if (canvasGroup == null)
            return;

        canvasGroup.alpha = enabled ? 1f : disabledAlpha;
        canvasGroup.interactable = enabled;
        canvasGroup.blocksRaycasts = enabled;

        if (button != null)
            button.interactable = enabled;
    }

    void Subscribe()
    {
        if (subscribed || PartyManager.Instance == null)
            return;

        PartyManager.Instance.OnSelectedUnitChanged += HandleSelectedUnitChanged;
        PartyManager.Instance.OnDetailsUnitChanged += HandleSelectedUnitChanged;
        subscribed = true;
    }

    void HandleSelectedUnitChanged(UnitData unit)
    {
        Refresh();
    }

    string GetOccupiedLabel()
    {
        if (!string.IsNullOrWhiteSpace(occupiedLabel))
            return occupiedLabel;

        return action == PreparationAction.Edit ? "Edit" : "Remove";
    }
}
