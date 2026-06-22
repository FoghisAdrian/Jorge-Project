using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelExit : MonoBehaviour
{
    [Header("Transition UI")]
    [SerializeField] private CanvasGroup fadeCanvasGroup; 
    [SerializeField] private float fadeDuration = 1.5f; 
    [SerializeField] private string nextSceneName;

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTransitioning)
        {
            StartCoroutine(TransitionToNextLevel(collision.gameObject));
        }
    }

    private IEnumerator TransitionToNextLevel(GameObject playerObject)
    {
        isTransitioning = true;

        JorgeMovement playerMovement = playerObject.GetComponent<JorgeMovement>();
        if (playerMovement != null)
        {
            playerMovement.horizontalMove = 0f;
            playerMovement.jump = false;
            playerMovement.crouch = false;

            if (playerMovement.jorgeAnimator != null)
            {
                playerMovement.jorgeAnimator.SetFloat("Speed", 0f);
                playerMovement.jorgeAnimator.SetBool("Jumping", false);
                playerMovement.jorgeAnimator.SetBool("IsCrouching", false);
            }

            playerMovement.enabled = false;
        }

        Rigidbody2D playerRb = playerObject.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.bodyType = RigidbodyType2D.Kinematic;
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                enemyRb.linearVelocity = Vector2.zero;
                enemyRb.bodyType = RigidbodyType2D.Kinematic;
            }
        }

        float counter = 0f;
        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;
            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, counter / fadeDuration);
            }
            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
