using UnityEngine;

public class PartyCharacterView : MonoBehaviour
{
    public UnitData data;

    [Header("Visual")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    [Header("Runtime")]
    public PartySlot currentSlot;

    //public void Setup(UnitData unitData)
    //{
    //    data = unitData;

    //    spriteRenderer.sprite = data.sprite;

    //    if (data.animator != null)
    //        animator.runtimeAnimatorController = data.animator;
    //}

    //public void SetSlot(PartySlot slot)
    //{
    //    currentSlot = slot;
    //}

    //void OnMouseDown()
    //{
    //    PartyUIManager.Instance.OpenCharacterMenu(this);
    //}
}

