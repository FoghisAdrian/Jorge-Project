using UnityEngine;

public class OneUpPerk : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            bool collected = GameManager.Instance.TryAddLife();

            if (collected)
            {
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Jorge has got the maximul of 5 lives already!");
            }
        }
    }
}
