using UnityEngine;

public class PartyCharacterView : MonoBehaviour
{
    public UnitData data;

    [Header("Visual")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    [SerializeField] private Vector2 colliderSizeMultiplier = new Vector2(0.85f, 0.9f);
    [SerializeField] private Color hoverColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private float selectedScaleMultiplier = 1.12f;
    [SerializeField] private float hoverScaleMultiplier = 1.06f;
    [SerializeField] private bool useSelectionOutline = true;
    [SerializeField] private Color outlineColor = new Color(1f, 0.75f, 0.1f, 0.9f);
    [SerializeField] private float outlineScaleMultiplier = 1.08f;

    [Header("Runtime")]
    public PartySlot currentSlot;
    public bool isRosterView;

    private CharacterDragHandler dragHandler;
    private SpriteRenderer outlineRenderer;
    private Vector3 baseScale;
    private bool isSelected;
    private bool isHovered;

    void Awake()
    {
        dragHandler = GetComponent<CharacterDragHandler>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        EnsureOutlineRenderer();
        baseScale = transform.localScale;
    }

    public void Setup(UnitData unitData, PartySlot slot = null, bool rosterView = false)
    {
        data = unitData;
        isRosterView = rosterView;
        currentSlot = slot;

        if (spriteRenderer != null && data != null)
            spriteRenderer.sprite = data.sprite;

        EnsureOutlineRenderer();
        SyncOutlineSprite();

        if (dragHandler != null)
            dragHandler.Configure(this);

        baseScale = transform.localScale;
        SetSelectedVisual(false);
        FitColliderToSprite();
    }

    public void SetSlot(PartySlot slot)
    {
        currentSlot = slot;
    }

    public void SetSelectedVisual(bool selected)
    {
        isSelected = selected;
        ApplyVisualState();
    }

    public void SetHoverVisual(bool hovered)
    {
        isHovered = hovered;
        ApplyVisualState();
    }

    void ApplyVisualState()
    {
        if (spriteRenderer != null)
        {
            if (isHovered && !isSelected)
                spriteRenderer.color = hoverColor;
            else
                spriteRenderer.color = normalColor;
        }

        float scaleMultiplier = 1f;

        if (isSelected)
            scaleMultiplier = selectedScaleMultiplier;
        else if (isHovered)
            scaleMultiplier = hoverScaleMultiplier;

        transform.localScale = baseScale * scaleMultiplier;

        if (outlineRenderer != null)
            outlineRenderer.gameObject.SetActive(isSelected && useSelectionOutline);
    }

    void EnsureOutlineRenderer()
    {
        if (!useSelectionOutline || outlineRenderer != null)
            return;

        GameObject outlineObject = new GameObject("SelectionOutline");
        outlineObject.transform.SetParent(transform, false);

        outlineRenderer = outlineObject.AddComponent<SpriteRenderer>();
        outlineRenderer.color = outlineColor;
        outlineRenderer.sortingOrder = spriteRenderer != null ? spriteRenderer.sortingOrder - 1 : -1;
        outlineObject.transform.localScale = Vector3.one * outlineScaleMultiplier;
        outlineObject.SetActive(false);
    }

    void SyncOutlineSprite()
    {
        if (outlineRenderer == null || spriteRenderer == null)
            return;

        outlineRenderer.sprite = spriteRenderer.sprite;
        outlineRenderer.flipX = spriteRenderer.flipX;
        outlineRenderer.flipY = spriteRenderer.flipY;
        outlineRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        outlineRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
    }

    void FitColliderToSprite()
    {
        if (spriteRenderer == null || spriteRenderer.sprite == null)
            return;

        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
            boxCollider = gameObject.AddComponent<BoxCollider2D>();

        Bounds spriteBounds = spriteRenderer.bounds;
        Vector3 localCenter = transform.InverseTransformPoint(spriteBounds.center);
        Vector3 localSize = transform.InverseTransformVector(spriteBounds.size);

        boxCollider.offset = localCenter;
        boxCollider.size = new Vector2(
            Mathf.Abs(localSize.x) * colliderSizeMultiplier.x,
            Mathf.Abs(localSize.y) * colliderSizeMultiplier.y
        );
    }
}
