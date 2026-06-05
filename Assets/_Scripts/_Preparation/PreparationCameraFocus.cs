using UnityEngine;

public class PreparationCameraFocus : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float focusedOrthographicSize = 2.2f;
    [SerializeField] private Vector2 focusedViewportPosition = new Vector2(0.25f, 0.5f);
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float zoomSpeed = 8f;

    private Vector3 defaultPosition;
    private float defaultOrthographicSize;
    private bool hasDefaultState;
    private bool subscribed;
    private Transform focusTarget;
    private bool isFocused;

    void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null)
            targetCamera = FindAnyObjectByType<Camera>();

        SaveDefaultState();
    }

    void OnEnable()
    {
        Subscribe();
    }

    void Start()
    {
        Subscribe();
        HandleSelectedUnitChanged(PartyManager.Instance != null ? PartyManager.Instance.DetailsUnit : null);
    }

    void OnDisable()
    {
        if (PartyManager.Instance != null && subscribed)
            PartyManager.Instance.OnDetailsUnitChanged -= HandleSelectedUnitChanged;

        subscribed = false;
    }

    void LateUpdate()
    {
        if (targetCamera == null || !hasDefaultState)
            return;

        float desiredSize = isFocused ? focusedOrthographicSize : defaultOrthographicSize;
        Vector3 desiredPosition = isFocused && focusTarget != null
            ? GetCameraPositionForFocus(focusTarget.position, desiredSize)
            : defaultPosition;

        targetCamera.orthographicSize = Mathf.Lerp(
            targetCamera.orthographicSize,
            desiredSize,
            Time.deltaTime * zoomSpeed
        );

        targetCamera.transform.position = Vector3.Lerp(
            targetCamera.transform.position,
            desiredPosition,
            Time.deltaTime * moveSpeed
        );
    }

    void Subscribe()
    {
        if (subscribed || PartyManager.Instance == null)
            return;

        PartyManager.Instance.OnDetailsUnitChanged += HandleSelectedUnitChanged;
        subscribed = true;
    }

    void HandleSelectedUnitChanged(UnitData unit)
    {
        if (PartyManager.Instance == null || unit == null)
        {
            focusTarget = null;
            isFocused = false;
            return;
        }

        PartyCharacterView selectedView = PartyManager.Instance.GetSelectedCharacterView();
        focusTarget = selectedView != null ? selectedView.transform : null;
        isFocused = focusTarget != null;
    }

    void SaveDefaultState()
    {
        if (targetCamera == null)
            return;

        defaultPosition = targetCamera.transform.position;
        defaultOrthographicSize = targetCamera.orthographicSize;
        hasDefaultState = true;
    }

    Vector3 GetCameraPositionForFocus(Vector3 targetPosition, float orthographicSize)
    {
        float height = orthographicSize * 2f;
        float width = height * targetCamera.aspect;

        Vector3 offsetFromCameraCenter = new Vector3(
            (focusedViewportPosition.x - 0.5f) * width,
            (focusedViewportPosition.y - 0.5f) * height,
            0f
        );

        return new Vector3(
            targetPosition.x - offsetFromCameraCenter.x,
            targetPosition.y - offsetFromCameraCenter.y,
            defaultPosition.z
        );
    }
}
