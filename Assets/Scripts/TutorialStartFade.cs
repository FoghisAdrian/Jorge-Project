using UnityEngine;
using System.Collections;

public class TutorialStartFade : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float delayBeforeStart = 0.5f; // Scurta pauza la spawn
    [SerializeField] private float fadeDuration = 0.6f;     // Cat de repede apar/dispar
    [SerializeField] private float visibleDuration = 3.5f;  // Cat timp stau vizibile

    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            StartCoroutine(StartTutorialSequence());
        }
        else
        {
            Debug.LogError("ERROR: No SpriteRenderer found on " + gameObject.name);
        }
    }

    private IEnumerator StartTutorialSequence()
    {
        yield return new WaitForSeconds(delayBeforeStart);
        yield return StartCoroutine(FadeSprite(0f, 1f));
        yield return new WaitForSeconds(visibleDuration);
        yield return StartCoroutine(FadeSprite(1f, 0f));
        gameObject.SetActive(false);
    }

    private IEnumerator FadeSprite(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color color = sr.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            color.a = newAlpha;
            sr.color = color;
            yield return null;
        }

        color.a = endAlpha;
        sr.color = color;
    }
}
