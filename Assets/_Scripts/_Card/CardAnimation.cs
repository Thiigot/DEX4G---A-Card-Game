using UnityEngine;

public class CardAnimation : MonoBehaviour
{
    private RectTransform rectTransform;

    public float moveSpeed = 8f;
    public float scaleSpeed = 8f;

    private Vector3 targetPos;
    private Vector3 targetScale;

    private bool animating = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void PlayDrawAnimation()
    {
        targetPos = rectTransform.localPosition;
        targetScale = Vector3.one;

        rectTransform.localScale = Vector3.one * 0.8f;
        animating = true;
    }

    void Update()
    {
        if (!animating) return;

        rectTransform.localPosition = Vector3.Lerp(rectTransform.localPosition, targetPos, Time.deltaTime * moveSpeed);
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, Time.deltaTime * scaleSpeed);

        if (Vector3.Distance(rectTransform.localScale, targetScale) < 0.01f)
        {
            animating = false;
        }
    }
}
