using UnityEngine;
using System.Collections;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private GameObject tutorialButtonToShow;
    [SerializeField] private GameObject buttonToHide;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.6f;
    [SerializeField] private float visibleDuration = 2.5f; 

    private Collider2D triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (triggerCollider != null) triggerCollider.enabled = false;

            StartCoroutine(TutorialSequence());
        }
    }

    private IEnumerator TutorialSequence()
    {
        if (buttonToHide != null)
        {
            SpriteRenderer hideSr = buttonToHide.GetComponent<SpriteRenderer>();
            if (hideSr != null) StartCoroutine(FadeSprite(hideSr, 1f, 0f));
        }

        if (tutorialButtonToShow != null)
        {
            SpriteRenderer showSr = tutorialButtonToShow.GetComponent<SpriteRenderer>();
            if (showSr != null)
            {
                tutorialButtonToShow.SetActive(true);
                yield return StartCoroutine(FadeSprite(showSr, 0f, 1f));

                yield return new WaitForSeconds(visibleDuration);

                yield return StartCoroutine(FadeSprite(showSr, 1f, 0f));
                tutorialButtonToShow.SetActive(false);
            }
        }

        Destroy(gameObject);
    }

    private IEnumerator FadeSprite(SpriteRenderer sr, float startAlpha, float endAlpha)
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
