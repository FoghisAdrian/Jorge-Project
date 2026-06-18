using UnityEngine;
using System.Collections;

public class BullEnemyAI : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;
    private Transform currentTarget;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chargeSpeed = 10f;
    public float chargeDistance = 7f;
    public float detectionRadius = 5f;

    [Header("Logic")]
    public Transform player;
    public LayerMask playerLayer;
    private bool isCharging = false;
    private bool isAngry = false;
    private bool isStunned = false;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        currentTarget = pointB;
    }

    void Update()
    {
        if (isCharging || isAngry || isStunned) return;

        DetectPlayer();
        Patrol();
    }

    void Patrol()
    {
        anim.SetBool("isWalking", true);
        anim.SetBool("isRunning", false);

        transform.position = Vector2.MoveTowards(transform.position, new Vector2(currentTarget.position.x, transform.position.y), patrolSpeed * Time.deltaTime);

        if (currentTarget.position.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);

        if (Mathf.Abs(transform.position.x - currentTarget.position.x) < 0.35f)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }
    }

    void DetectPlayer()
    {
        if (player == null) return;

        float heightDifference = Mathf.Abs(transform.position.y - player.position.y);

        if (heightDifference > 1.5f)
        {
            return;
        }

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer < detectionRadius && !isAngry && !isStunned)
        {
            float directionToPlayer = player.position.x - transform.position.x;
            bool isFacingPlayer = (transform.localScale.x > 0 && directionToPlayer > 0) || (transform.localScale.x < 0 && directionToPlayer < 0);

            if (isFacingPlayer)
            {
                StartCoroutine(AngerSequence());
            }
        }
    }

    IEnumerator AngerSequence()
    {
        isAngry = true;

        anim.SetBool("isWalking", false);
        anim.SetBool("isRunning", false);

        anim.Play("BullMaskGangMemberIdle");

        Color originalColor = sr.color;
        Vector3 startPos = transform.position;

        for (int i = 0; i < 10; i++)
        {
            sr.color = Color.red;
            transform.position += new Vector3(Random.Range(-0.06f, 0.06f), 0, 0);

            yield return new WaitForSeconds(0.05f);

            transform.position = new Vector3(startPos.x, transform.position.y, transform.position.z);
            sr.color = originalColor;

            yield return new WaitForSeconds(0.05f);
        }

        sr.color = Color.red;
        StartCoroutine(Charge());
    }

    IEnumerator Charge()
    {
        isCharging = true;
        anim.SetBool("isRunning", true);

        float chargeDir = transform.localScale.x;
        float traveled = 0;

        while (traveled < chargeDistance)
        {
            float move = chargeDir * chargeSpeed * Time.deltaTime;
            transform.Translate(move, 0, 0);
            traveled += Mathf.Abs(move);
            yield return null;
        }

        isCharging = false;
        anim.SetBool("isRunning", false);
        anim.SetBool("isWalking", false);
        sr.color = Color.white;

        isStunned = true;
        yield return new WaitForSeconds(2.0f);
        isStunned = false;
        isAngry = false;

        if (player != null)
        {
            float directionToPlayer = player.position.x - transform.position.x;

            if (directionToPlayer > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }

            float distToA = Mathf.Abs(transform.position.x - pointA.position.x);
            float distToB = Mathf.Abs(transform.position.x - pointB.position.x);

            if (directionToPlayer > 0)
            {
                currentTarget = (pointB.position.x > pointA.position.x) ? pointB : pointA;
            }
            else
            {
                currentTarget = (pointB.position.x < pointA.position.x) ? pointB : pointA;
            }
        }
    }

    public void HandleDeath()
    {
        StopAllCoroutines();
        sr.color = Color.white; 
        this.enabled = false; 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isCharging)
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null) playerHealth.TakeDamage(2);

            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockbackDir = new Vector2(transform.localScale.x * 6f, 9f);
                playerRb.linearVelocity = Vector2.zero;
                playerRb.AddForce(knockbackDir, ForceMode2D.Impulse);
            }
        }
    }
}