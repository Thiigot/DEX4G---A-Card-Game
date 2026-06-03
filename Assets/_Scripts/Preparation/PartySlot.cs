using UnityEngine;

public class PartySlot : MonoBehaviour
{
    public int index;
    public PartyCharacterView currentCharacter;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer highlightRenderer;
    [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 0.25f);
    [SerializeField] private Color hoverColor = new Color(0.4f, 0.9f, 1f, 0.65f);

    void Awake()
    {
        if (highlightRenderer == null)
            highlightRenderer = GetComponent<SpriteRenderer>();

        SetHighlight(false);
    }

    public bool IsEmpty()
    {
        return currentCharacter == null;
    }

    public void SetCharacter(PartyCharacterView character)
    {
        currentCharacter = character;
        character.SetSlot(this);

        character.transform.SetParent(transform, false);
        character.transform.localPosition = Vector3.zero;
    }

    public UnitData GetUnit()
    {
        return currentCharacter != null ? currentCharacter.data : null;
    }

    public void SetHighlight(bool value)
    {
        if (highlightRenderer == null) return;

        highlightRenderer.color = value ? hoverColor : normalColor;
    }

    public void Clear()
    {
        currentCharacter = null;
    }

    public void ClearAndDestroyView()
    {
        if (currentCharacter != null)
            Destroy(currentCharacter.gameObject);

        currentCharacter = null;
    }
}
