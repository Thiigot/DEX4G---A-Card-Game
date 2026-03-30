using UnityEngine;
using UnityEngine.UI;

public class CardDiscardAnimation : MonoBehaviour
{
    private Vector3 target;
    private float speed = 8f;

    private CanvasGroup canvasGroup;

    public void StartDiscard(Vector3 targetPos)
    {
        target = targetPos;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Update()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            target,
            Time.deltaTime * speed
        );

        if (canvasGroup != null)
        {
            canvasGroup.alpha = Mathf.Lerp(
                canvasGroup.alpha,
                0f,
                Time.deltaTime * speed
            );
        }

        if (Vector3.Distance(transform.position, target) < 10f)
        {
            gameObject.SetActive(false);
        }
    }
}