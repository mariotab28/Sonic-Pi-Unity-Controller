using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockIndicatorController : MonoBehaviour
{
    //TODO: [RequireComponent]
    CanvasGroup canvasGroup;

    [SerializeField] float fadeDuration = 0.5f;
    bool fading = false;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup) Debug.LogError("Error: No Canvas Group component in indicator.");
    }

    public void ShowIndicator()
    {
        fading = false;
        canvasGroup.alpha = 1.0f;
    }

    public void HideIndicator()
    {
        fading = true;
        StartCoroutine(FadeIndicator(fadeDuration));
    }

    IEnumerator FadeIndicator(float duration)
    {
        float time = 0;
        float end = 0.0f;
        float start = canvasGroup.alpha;
        while (time < duration && fading)
        {
            canvasGroup.alpha = Mathf.Lerp(start, end, time / duration);
            time += Time.deltaTime;

            yield return null;
        }
        if(fading) canvasGroup.alpha = 0.0f;
    }
}
