using UnityEngine;

public class CharacterDragHandler : MonoBehaviour
{
    private Vector3 offset;
    private PartySlot originalSlot;

    //void OnMouseDown()
    //{
    //    originalSlot = GetComponent<Unit>().CurrentSlot;
    //    offset = transform.position - MouseWorld();
    //}

    //void OnMouseDrag()
    //{
    //    transform.position = MouseWorld() + offset;
    //}

    //void OnMouseUp()
    //{
    //    PartySlot target = FindClosestSlot();

    //    if (target != null)
    //    {
    //        SwapSlots(target);
    //    }
    //    else
    //    {
    //        transform.position = originalSlot.transform.position;
    //    }
    //}

    Vector3 MouseWorld()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        return pos;
    }
}