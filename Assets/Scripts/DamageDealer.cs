using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] private int damageValue = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Health enemyHealth = collision.GetComponent<Health>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damageValue);
        }
    }
}