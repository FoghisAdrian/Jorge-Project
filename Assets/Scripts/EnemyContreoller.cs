using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform[] patrolPoints;

    public Animator enemyAnimator;

    private int currentPointIndex;
    private Rigidbody2D rb;
    private bool facingRight = false;

    public GameObject churroPrefab;
    public Transform throwPoint;
    [SerializeField] private float timeBetweenAttacks = 2f;
    private float nextAttackTime;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private float detectionRange = 10f; 
    [SerializeField] private float stopDistance = 4f; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (enemyAnimator == null)
        {
            enemyAnimator = GetComponent<Animator>();
        }
        currentPointIndex = 0;
    }

    void FixedUpdate()
    {
        if (playerTransform == null) return;

        float heightDifference = Mathf.Abs(transform.position.y - playerTransform.position.y);

        if (heightDifference > 1.5f)
        {
            Patrol();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer < detectionRange)
        {
            HandleCombat(distanceToPlayer);
        }
        else
        {
            Patrol();
        }
    }

    private void HandleCombat(float distance)
    {
        float dirToPlayer = playerTransform.position.x - transform.position.x;

        if (dirToPlayer > 0 && !facingRight) Flip();
        else if (dirToPlayer < 0 && facingRight) Flip();

        if (distance > stopDistance)
        {
            float moveDirX = (dirToPlayer > 0) ? 1 : -1;
            rb.linearVelocity = new Vector2(moveDirX * moveSpeed, rb.linearVelocity.y);
            enemyAnimator.SetBool("IsWalking", true);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            enemyAnimator.SetBool("IsWalking", false);

            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + timeBetweenAttacks;
            }
        }
    }

    private void Patrol()
    {
        Vector2 targetPoint = patrolPoints[currentPointIndex].position;
        float distanceX = Mathf.Abs(transform.position.x - targetPoint.x);

        if (distanceX < 0.2f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            targetPoint = patrolPoints[currentPointIndex].position;
        }

        float moveDirX = (targetPoint.x > transform.position.x) ? 1 : -1;
        rb.linearVelocity = new Vector2(moveDirX * moveSpeed, rb.linearVelocity.y);

        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.01f;
        if (enemyAnimator != null)
        {
            enemyAnimator.SetBool("IsWalking", isMoving);
        }

        if (moveDirX < 0 && facingRight) Flip();
        else if (moveDirX > 0 && !facingRight) Flip();
    }

    private void Attack()
    {
        enemyAnimator.SetTrigger("Attack");
    }

    public void ThrowChurro()
    {
        GameObject newChurro = Instantiate(churroPrefab, throwPoint.position, throwPoint.rotation);

        if (!facingRight)
        {
            newChurro.transform.right = Vector2.left;
        }
        else
        {
            newChurro.transform.right = Vector2.right;
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
