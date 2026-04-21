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

            if (GetComponent<EnemyController>() != null)
            {
                GetComponent<EnemyController>().enabled = false;
            }

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