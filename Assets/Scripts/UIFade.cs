using System.Collections;
using UnityEngine;

public class UIFade : MonoBehaviour
{
    public CanvasGroup fadeOut;
    public CanvasGroup fadeIn;

    private void Start()
    {
        fadeOut.alpha = 1f;
        fadeIn.alpha = 0f;

        StartCoroutine(Fades());
    }

    private IEnumerator Fades()
    {
        yield return StartCoroutine(Fade(fadeIn, 0f, 1f, 0.75f));
        yield return new WaitForSeconds(1.6f);
        yield return StartCoroutine(Fade(fadeOut, 1f, 0f, 0.75f));

        gameObject.SetActive(false);
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float startAlpha, float targetAlpha, float duration = -1.0f)
    {
        float timer = 0f;

        while (timer < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            canvasGroup.alpha = alpha;

            timer += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    public void Skip()
    {
        StopAllCoroutines();

        fadeOut.alpha = 0f;

        gameObject.SetActive(false);
    }
}
