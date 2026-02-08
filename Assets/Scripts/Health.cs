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

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log(gameObject.name + " took damage! HP: " + currentHealth);

        OnDamageTaken.Invoke();
        StartCoroutine(FlashRed());

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
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        GetComponent<Collider2D>().enabled = false;

        if (GetComponent<EnemyController>() != null)
        {
            GetComponent<EnemyController>().enabled = false;
        }

        Destroy(gameObject, 1.5f);
    }
}