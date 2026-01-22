using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Movement and Patrol Settings
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform[] patrolPoints;

    // NEW: Animator reference
    public Animator enemyAnimator;

    private int currentPointIndex;
    private Rigidbody2D rb;
    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Get the Animator component if not assigned in the Inspector
        if (enemyAnimator == null)
        {
            enemyAnimator = GetComponent<Animator>();
        }
        currentPointIndex = 0;
    }

    void FixedUpdate()
    {
        Patrol();
    }

    private void Patrol()
    {
        // 1. Calculate direction to the next patrol point
        Vector2 targetPoint = patrolPoints[currentPointIndex].position;
        Vector2 moveDirection = (targetPoint - (Vector2)transform.position).normalized;

        // 2. Move the enemy
        rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, rb.linearVelocity.y);

        // 3. NEW: Animation Logic
        // Check if the enemy is moving horizontally.
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.01f;

        // Update the Animator parameter
        if (enemyAnimator != null)
        {
            enemyAnimator.SetBool("IsWalking", isMoving);
        }

        // 4. Check if the enemy has reached the current patrol point
        float distance = Vector2.Distance(transform.position, targetPoint);

        if (distance < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }

        // 5. Handle flipping the sprite
        if (moveDirection.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveDirection.x < 0 && facingRight)
        {
            Flip();
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
