using UnityEngine;

public class Churro : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 1;
    public float knockbackForce = 5f;

    private Vector2 flyDirection;
    private bool directionSet = false;

    void Start()
    {
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        if (!directionSet)
        {
            flyDirection = transform.right;
            directionSet = true;
        }

        transform.position += (Vector3)flyDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Churro Hit Jorge!");

            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);

                if (CameraShake.Instance != null)
                {
                    CameraShake.Instance.Shake(5f, 0.1f);
                }
            }

            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 direction = (collision.transform.position - transform.position).normalized;
                playerRb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
            }

            Destroy(gameObject);
        }
    }
}
