using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform[] patrolPoints;

    public Animator enemyAnimator;

    private int currentPointIndex;
    private Rigidbody2D rb;
    private bool facingRight = true;

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
        Patrol();
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

        if (moveDirX < 0 && !facingRight) Flip();
        else if (moveDirX > 0 && facingRight) Flip();
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
