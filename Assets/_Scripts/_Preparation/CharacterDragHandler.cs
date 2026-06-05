using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float slotSnapMaxDistance = 2.5f;

    private Vector3 startPosition;
    private PartyCharacterView view;
    private PartySlot hoveredSlot;
    private SpriteRenderer spriteRenderer;
    private int originalSortingOrder;
    private bool isDragging;

    public void Configure(PartyCharacterView characterView)
    {
        view = characterView;
        spriteRenderer = view != null ? view.spriteRenderer : GetComponentInChildren<SpriteRenderer>();
    }

    void Awake()
    {
        if (view == null)
            view = GetComponent<PartyCharacterView>();

        spriteRenderer = view != null ? view.spriteRenderer : GetComponentInChildren<SpriteRenderer>();

        Collider2D collider2D = GetComponent<Collider2D>();
        if (collider2D == null)
            gameObject.AddComponent<BoxCollider2D>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (view == null || view.data == null) return;

        Debug.Log($"Started dragging preparation character: {view.data.unitName}");

        startPosition = transform.position;
        MoveVisualCenterToPointer(eventData.position);

        if (spriteRenderer != null)
        {
            originalSortingOrder = spriteRenderer.sortingOrder;
            spriteRenderer.sortingOrder = 100;
        }

        if (!view.isRosterView && PartyManager.Instance != null && PartyManager.Instance.SelectedUnit == view.data)
            PartyManager.Instance.Select(null);

        isDragging = true;
        view.SetHoverVisual(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveVisualCenterToPointer(eventData.position);
        UpdateHoveredSlot();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (spriteRenderer != null)
            spriteRenderer.sortingOrder = originalSortingOrder;

        isDragging = false;

        PartySlot target = FindClosestSlot();
        ClearHoveredSlot();

        if (target != null)
        {
            bool moved = PartyManager.Instance.MoveToSlot(view.data, target.index);

            if (moved && view.isRosterView)
            {
                transform.position = startPosition;
                return;
            }

            if (moved)
                return;

            transform.position = startPosition;
            return;
        }

        PartyRosterDropArea rosterDropArea = FindClosestRosterDropArea();

        if (!view.isRosterView && rosterDropArea != null)
        {
            PartyManager.Instance.Remove(view.data);
            return;
        }

        transform.position = startPosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (view == null || view.data == null || view.isRosterView) return;
        if (PartyManager.Instance == null) return;

        if (PartyManager.Instance.SelectedUnit == view.data)
            PartyManager.Instance.Select(null);
        else
            PartyManager.Instance.Select(view.data);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (view == null || isDragging) return;

        view.SetHoverVisual(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (view == null || isDragging) return;

        view.SetHoverVisual(false);
    }

    void MoveVisualCenterToPointer(Vector2 screenPosition)
    {
        Vector3 pointerWorld = ScreenToWorld(screenPosition);

        if (spriteRenderer == null)
        {
            transform.position = pointerWorld;
            return;
        }

        Vector3 visualCenter = spriteRenderer.bounds.center;
        Vector3 delta = pointerWorld - visualCenter;
        transform.position += delta;
    }

    Vector3 ScreenToWorld(Vector2 screenPosition)
    {
        Camera camera = Camera.main;

        if (camera == null)
            camera = FindAnyObjectByType<Camera>();

        if (camera == null)
            return transform.position;

        float distanceFromCamera = Mathf.Abs(camera.transform.position.z - transform.position.z);
        Vector3 screenPoint = new Vector3(screenPosition.x, screenPosition.y, distanceFromCamera);
        Vector3 worldPoint = camera.ScreenToWorldPoint(screenPoint);
        worldPoint.z = transform.position.z;

        return worldPoint;
    }

    PartySlot FindClosestSlot()
    {
        PartySlot closest = null;
        float closestDistance = slotSnapMaxDistance;

        foreach (PartySlot slot in FindObjectsByType<PartySlot>(FindObjectsSortMode.None))
        {
            float distance = Vector2.Distance(transform.position, slot.transform.position);

            if (distance < closestDistance)
            {
                closest = slot;
                closestDistance = distance;
            }
        }

        return closest;
    }

    void UpdateHoveredSlot()
    {
        PartySlot closest = FindClosestSlot();

        if (closest == hoveredSlot) return;

        ClearHoveredSlot();
        hoveredSlot = closest;

        if (hoveredSlot != null)
            hoveredSlot.SetHighlight(true);
    }

    void ClearHoveredSlot()
    {
        if (hoveredSlot != null)
            hoveredSlot.SetHighlight(false);

        hoveredSlot = null;
    }

    PartyRosterDropArea FindClosestRosterDropArea()
    {
        foreach (PartyRosterDropArea dropArea in FindObjectsByType<PartyRosterDropArea>(FindObjectsSortMode.None))
        {
            if (dropArea.IsInside(transform.position))
                return dropArea;
        }

        return null;
    }
}
