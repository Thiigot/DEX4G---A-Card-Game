using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    Camera cam;

    Vector3 originalPos;
    float originalSize;

    Coroutine currentRoutine;

    void Awake()
    {
        Instance = this;
        cam = Camera.main;
    }

    public void FocusOnUnit(Unit unit, bool isEnemy)
    {
        if (unit == null) return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        originalPos = cam.transform.position;
        originalSize = cam.orthographicSize;

        Vector3 targetPos = unit.transform.position;

        // 🔥 offset lateral (pra não ficar atrás do painel)
        float offsetX = isEnemy ? -2f : 2f;

        targetPos += new Vector3(offsetX, 0f, -10f);

        float zoomSize = originalSize * 0.6f;

        currentRoutine = StartCoroutine(AnimateCamera(targetPos, zoomSize));
    }

    public void ResetCamera()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(AnimateCamera(originalPos, originalSize));
    }

    IEnumerator AnimateCamera(Vector3 targetPos, float targetSize)
    {
        float t = 0;
        float duration = 0.3f;

        Vector3 startPos = cam.transform.position;
        float startSize = cam.orthographicSize;

        while (t < 1)
        {
            t += Time.deltaTime / duration;

            cam.transform.position = Vector3.Lerp(startPos, targetPos, t);
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, t);

            yield return null;
        }
    }
}