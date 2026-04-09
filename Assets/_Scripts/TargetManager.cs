using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class TargetManager : MonoBehaviour
{
    public static TargetManager Instance;

    private Action<BoardSlot> onTargetSelected;
    private System.Action onCancel;
    public static bool isTargeting = false;
    [SerializeField] private HandManager handManager;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!isTargeting) return;
        UpdateSlotHover();

        if (CancelPressed())
        {
            CancelTargeting();
        }
            
    }
    void UpdateSlotHover()
{
    foreach (var slot in FindObjectsByType<BoardSlot>(FindObjectsSortMode.None))
    {
        bool isHovering = RectTransformUtility.RectangleContainsScreenPoint(
            slot.transform as RectTransform,
            Mouse.current.position.ReadValue()
        );

        slot.SetHover(isHovering);
    }
}
    public void StartTargeting(System.Action<BoardSlot> onSelected, System.Action onCancelCallback = null)
    {
        isTargeting = true;
        onTargetSelected = onSelected;
        onCancel = onCancelCallback;
    }

    public void SelectTarget(BoardSlot slot)
    {

        if (!isTargeting) return;

        onTargetSelected?.Invoke(slot);

        EndTargeting();
    }

    void EndTargeting()
    {
        // 🔥 limpa hover de todos os slots
        foreach (var slot in UnityEngine.Object.FindObjectsByType<BoardSlot>(FindObjectsSortMode.None))
        {
            slot.SetHover(false);
        }

        isTargeting = false;

        onTargetSelected = null;
        onCancel = null;

        // 🔥 libera o jogo novamente
        PlayArea.HasCardInPlay = false;
    }

    public bool IsTargeting()
    {
        return onTargetSelected != null;
    }

    public void CancelTargeting()
    {
        if (!isTargeting) return;

        onCancel?.Invoke();

        EndTargeting();
    }

    bool CancelPressed()
    {
        bool mouseCancel = Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame;
        bool keyboardCancel = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;

        
        return mouseCancel || keyboardCancel;
    }
}