using UnityEngine;
using System;

public class TargetManager : MonoBehaviour
{
    public static TargetManager Instance;

    private Action<BoardSlot> onTargetSelected;

    void Awake()
    {
        Instance = this;
    }

    public void StartTargeting(Action<BoardSlot> callback)
    {
        onTargetSelected = callback;
    }

    public void SelectTarget(BoardSlot slot)
    {
        if (onTargetSelected != null)
        {
            onTargetSelected.Invoke(slot);
            onTargetSelected = null;
        }
    }

    public bool IsTargeting()
    {
        return onTargetSelected != null;
    }
}