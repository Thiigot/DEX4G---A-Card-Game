using System.Collections.Generic;
using UnityEngine;

public class PartyRosterUI : MonoBehaviour
{
    [SerializeField] private PartyCharacterView rosterCharacterPrefab;
    [SerializeField] private Transform rosterRoot;
    [SerializeField] private float spacing = 1.4f;
    [SerializeField] private bool hideCharactersAlreadyInParty = false;
    [SerializeField] private float unavailableAlpha = 0.35f;

    private readonly List<PartyCharacterView> spawnedViews = new();

    void OnEnable()
    {
        if (PartyManager.Instance != null)
            PartyManager.Instance.OnPartyChanged += RefreshAvailability;
    }

    void OnDisable()
    {
        if (PartyManager.Instance != null)
            PartyManager.Instance.OnPartyChanged -= RefreshAvailability;
    }

    void Start()
    {
        BuildRoster();
    }

    public void BuildRoster()
    {
        Clear();

        if (PartyManager.Instance == null || rosterCharacterPrefab == null)
            return;

        if (rosterRoot == null)
            rosterRoot = transform;

        List<UnitData> roster = PartyManager.Instance.roster;

        for (int i = 0; i < roster.Count; i++)
        {
            UnitData unit = roster[i];
            if (unit == null) continue;

            PartyCharacterView view = Instantiate(rosterCharacterPrefab, rosterRoot);
            view.Setup(unit, null, true);
            view.transform.localPosition = Vector3.right * spacing * i;

            spawnedViews.Add(view);
        }

        RefreshAvailability();
    }

    void RefreshAvailability()
    {
        foreach (PartyCharacterView view in spawnedViews)
        {
            if (view == null || view.spriteRenderer == null) continue;

            bool alreadyInParty = PartyManager.Instance.IsInParty(view.data);

            if (hideCharactersAlreadyInParty)
            {
                view.gameObject.SetActive(!alreadyInParty);
                continue;
            }

            view.gameObject.SetActive(true);

            Color color = view.spriteRenderer.color;
            color.a = alreadyInParty ? unavailableAlpha : 1f;
            view.spriteRenderer.color = color;

            CharacterDragHandler dragHandler = view.GetComponent<CharacterDragHandler>();
            if (dragHandler != null)
                dragHandler.enabled = !alreadyInParty;
        }
    }

    void Clear()
    {
        foreach (PartyCharacterView view in spawnedViews)
        {
            if (view != null)
                Destroy(view.gameObject);
        }

        spawnedViews.Clear();
    }
}
