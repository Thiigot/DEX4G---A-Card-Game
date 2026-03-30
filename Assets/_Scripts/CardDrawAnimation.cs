using UnityEngine;

public class CardDrawAnimation : MonoBehaviour
{
    private Vector3 targetPosition;
    private Transform targetParent;

    private float speed = 10f;
    private bool animating = false;

    public void StartAnimation(Vector3 targetPos, Transform parent)
    {
        targetPosition = targetPos;
        targetParent = parent;
        animating = true;
    }

    void Update()
    {
        if (!animating) return;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            Time.deltaTime * speed
        );

        if (Vector3.Distance(transform.position, targetPosition) < 5f)
        {
            transform.SetParent(targetParent, false);
            transform.localPosition = targetPosition;
            animating = false;
        }
    }
}