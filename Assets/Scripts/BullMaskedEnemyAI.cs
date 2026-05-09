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
        if (isCharging || isAngry) return;

        DetectPlayer();
        Patrol();
    }

    void Patrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, patrolSpeed * Time.deltaTime);
        anim.SetBool("isWalking", true);

        if (Vector2.Distance(transform.position, currentTarget.position) < 0.2f)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
            Flip();
        }
    }

    void DetectPlayer()
    {
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer < detectionRadius && !isAngry)
        {
            float directionToPlayer = player.position.x - transform.position.x;
            bool isFacingPlayer = (transform.localScale.x > 0 && directionToPlayer > 0) || (transform.localScale.x < 0 && directionToPlayer < 0);

            if (isFacingPlayer)
            {
                StartCoroutine(AngerSequence());
            }
        }
    }

    void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    IEnumerator AngerSequence()
    {
        isAngry = true;
        anim.SetBool("isWalking", false);

        Color originalColor = sr.color;
        for (int i = 0; i < 10; i++)
        {
            sr.color = Color.red;
            transform.position += new Vector3(0.05f, 0, 0);
            yield return new WaitForSeconds(0.05f);
            transform.position -= new Vector3(0.05f, 0, 0);
            sr.color = originalColor;
            yield return new WaitForSeconds(0.05f);
        }

        StartCoroutine(Charge());
    }

    IEnumerator Charge()
    {
        isCharging = true;
        anim.SetBool("isRunning", true);

        float chargeDir = transform.localScale.x;
        Vector2 startPos = transform.position;
        float traveled = 0;

        while (traveled < chargeDistance)
        {
            float move = chargeDir * chargeSpeed * Time.deltaTime;
            transform.Translate(move, 0, 0);
            traveled += Mathf.Abs(move);
            yield return null;
        }

        anim.SetBool("isRunning", false);
        yield return new WaitForSeconds(1f);
        isCharging = false;
        isAngry = false;
    }
}