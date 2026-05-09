using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    public UnityEvent OnDamageTaken;
    public UnityEvent OnDeath;

    [SerializeField] private Color damageColor = Color.red;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        originalColor = spriteRenderer.color;
    }

    public int GetCurrentHealth() { return currentHealth; }
    public int GetMaxHealth() { return maxHealth; }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log(gameObject.name + " took damage! HP: " + currentHealth);

        OnDamageTaken.Invoke();
        StartCoroutine(FlashRed());

        if (gameObject.CompareTag("Player") && GameManager.Instance != null)
        {
            GameManager.Instance.UpdateHearts(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashRed()
    {
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        if (!gameObject.CompareTag("Player"))
        {
            Animator anim = GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("Die");
            }

            // Disable the AI so he stops trying to move
            if (GetComponent<EnemyController>() != null)
            {
                GetComponent<EnemyController>().enabled = false;
            }

            // --- PHYSICS FIXES START HERE ---

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // 1. Switch to Dynamic so gravity pulls him down
                rb.bodyType = RigidbodyType2D.Dynamic;

                // 2. Unlock rotation so he can actually spin/flip
                rb.constraints = RigidbodyConstraints2D.None;

                // 3. Give him a tiny 'death hop' so he clears the platform edge
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 5f);

                // 4. Make him rotate as he falls
                rb.AddTorque(10f, ForceMode2D.Impulse);
            }

            // 5. Disable the collider so he slips THROUGH the floor and doesn't get stuck
            if (GetComponent<Collider2D>() != null)
            {
                GetComponent<Collider2D>().enabled = false;
            }

            // --- PHYSICS FIXES END HERE ---

            Destroy(gameObject, 1.5f);
        }
        else
        {
            Debug.Log("Jorge died! No animation yet, just respawning...");
            GetComponent<Collider2D>().enabled = false;
            gameObject.SetActive(false);
            GameManager.Instance.PlayerDied();
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateHearts(currentHealth);
        }
    }
}