using UnityEngine;
using TMPro;

public class WarningUI : MonoBehaviour
{
    public TMP_Text text;
    public CanvasGroup canvasGroup;

    public float fadeDuration = 0.25f;
    public float displayTime = 0.3f;

    private float timer;
    private bool showing = false;

    public void Show(string message)
    {
        text.text = message;

        canvasGroup.alpha = 1f;
        timer = displayTime;
        showing = true;
    }

    void Update()
    {
        if (!showing) return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            canvasGroup.alpha = Mathf.Lerp(
                canvasGroup.alpha,
                0f,
                Time.deltaTime * (1f / fadeDuration)
            );

            if (canvasGroup.alpha < 0.05f)
            {
                canvasGroup.alpha = 0f;
                showing = false;
            }
        }
    }
}