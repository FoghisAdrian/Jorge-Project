using UnityEngine;

public class OneUpPerk : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip oneUpSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            bool collected = GameManager.Instance.TryAddLife();

            if (collected)
            {
                if (oneUpSound != null)
                {
                    AudioSource.PlayClipAtPoint(oneUpSound, transform.position);
                }

                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Jorge has got the maximul of 5 lives already!");
            }
        }
    }
}
