using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    private PlayerInputActions input;
    [SerializeField] DeckUIManager deckUI;
    [SerializeField] WarningUI warningUI;

    public List<Unit> allUnits = new List<Unit>();
    public List<Unit> turnOrder = new List<Unit>();

    public int currentIndex = 0;

    public bool playerFinishedTurn = false;
    public Unit currentUnit;
    private float nextNavTime;

    void Awake()
    {
        Instance = this;

        input = new PlayerInputActions();

        input.Gameplay.EndTurn.performed += ctx => OnEndTurn();
        input.Gameplay.Confirm.performed += ctx => OnConfirm();
        input.Gameplay.Cancel.performed += ctx => OnCancel();
        input.Gameplay.Draw.performed += ctx => OnDraw();
    }

    void Start()
    {
        allUnits = FindObjectsOfType<Unit>().ToList();
        StartCoroutine(GameLoop());
    }

    void Update()
    {
        if (Time.time < nextNavTime) return;

        Vector2 nav = input.Gameplay.Navigate.ReadValue<Vector2>();

        if (nav.magnitude > 0.5f)
        {
            nextNavTime = Time.time + 0.2f;

            Debug.Log("NAV: " + nav);
        }
    }

    IEnumerator GameLoop()
    {
        while (true)
        {
            CalculateTurnOrder();
            currentIndex = 0;

            while (currentIndex < turnOrder.Count)
            {
                currentUnit = turnOrder[currentIndex];
                if (deckUI != null)
                {
                    deckUI.UpdateTurnText(currentUnit);
                    deckUI.UpdateUI();
                }
                if (warningUI != null)
                    warningUI.Show($"{currentUnit.unitName}'s Turn");

                yield return new WaitForSeconds(0.4f);

                yield return StartCoroutine(currentUnit.TakeTurn());
                currentIndex++;
            }
        }
    }

    void CalculateTurnOrder()
    {
        turnOrder = allUnits
            .OrderByDescending(u => u.speed)
            .ToList();
    }

    public void EndPlayerTurn()
    {
        playerFinishedTurn = true;
    }

    public bool IsPlayerTurn(Unit unit)
    {
        return currentUnit == unit && unit.isPlayer;
    }

    void OnEnable() => input.Enable();
    void OnDisable() => input.Disable();
    void OnEndTurn()
    {
        if (currentUnit != null && currentUnit.isPlayer)
        {
            EndPlayerTurn();
        }
    }
    void OnConfirm()
    {
        Debug.Log("CONFIRM");

        // exemplo: jogar carta selecionada
        // CardSelectionManager.Instance.PlaySelectedCard();
    }
    void OnCancel()
    {
        Debug.Log("CANCEL");

        // exemplo: cancelar target ou seleção
    }
    void OnDraw()
    {
        if (currentUnit != null && currentUnit.isPlayer)
        {
            var deck = currentUnit.GetComponent<DeckManager>();

            if (deck != null)
                deck.DrawCard();
            else
            {
                Debug.Log("Deck não encontrado");
            }
        }
    }
}