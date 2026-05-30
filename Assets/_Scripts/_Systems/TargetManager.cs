using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class TargetManager : MonoBehaviour
{
    public static TargetManager Instance;

    private Action<BoardSlot> onTargetSelected;
    private Action onCancel;

    public static bool isTargeting = false;

    [SerializeField] private GameObject targetTextUI;

    void Awake()
    {
        Instance = this;
        targetTextUI.SetActive(false);
    }

    void Update()
    {
        if (!isTargeting) return;

        //UpdateSlotHover();

        if (CancelPressed())
        {
            CancelTargeting();
        }
    }

    //void UpdateSlotHover()
    //{
    //    foreach (var slot in FindObjectsByType<BoardSlot>(FindObjectsSortMode.None))
    //    {
    //        bool isHovering = RectTransformUtility.RectangleContainsScreenPoint(
    //            slot.transform as RectTransform,
    //            Mouse.current.position.ReadValue()
    //        );

    //        slot.SetTargetHighlight(isHovering);
    //    }
    //}

    public void StartTargeting(Action<BoardSlot> onSelected, Action onCancelCallback = null)
    {
        isTargeting = true;
        onTargetSelected = onSelected;
        onCancel = onCancelCallback;

        targetTextUI.SetActive(true);
    }

    public void SelectTarget(BoardSlot slot)
    {
        if (!isTargeting)
        {
            return;
        }

        onTargetSelected?.Invoke(slot);
        EndTargeting();
    }

    void EndTargeting()
    {
        foreach (var slot in FindObjectsByType<BoardSlot>(FindObjectsSortMode.None))
        {
            slot.SetTargetHighlight(false);
        }
        isTargeting = false;

        onTargetSelected = null;
        onCancel = null;

        targetTextUI.SetActive(false);

        PlayArea.HasCardInPlay = false;
    }

    public bool IsTargeting()
    {
        return isTargeting;
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