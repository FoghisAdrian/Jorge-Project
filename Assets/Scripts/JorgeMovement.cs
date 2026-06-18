using UnityEngine;
using UnityEngine.InputSystem;

public class JorgeMovement : MonoBehaviour
{
    public JorgeController jorgeController;
    public float horizontalMove = 0f;
    public float runSpeed = 40f;
    public bool jump = false;
    public bool crouch = false;
    public Animator jorgeAnimator;
    public bool isAttacking = false;

    [Header("Guitar Smash Combat")]
    public int smashDamage = 3;
    public GameObject guitarHitBox;

    void Update()
    {
        bool isGrounded = jorgeController.m_Grounded;

        if (Input.GetKey(KeyCode.S))
        {
            crouch = true;
        }
        else
        {
            crouch = false;
        }

        jorgeAnimator.SetBool("IsCrouching", crouch);

        if (!isAttacking && isGrounded && !crouch && (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Fire1")) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            jorgeAnimator.SetTrigger("Attack");
            isAttacking = true;

            Debug.Log("DEBUG ATAC: Tasta de atac a fost apasata! Starea isAttacking este: " + isAttacking);
        }

        if (!isAttacking && isGrounded && !crouch && Input.GetKeyDown(KeyCode.Q))
        {
            jorgeAnimator.SetTrigger("GuitarSmash");
            isAttacking = true;
        }

        if (isAttacking)
        {
            horizontalMove = 0f;
            jorgeAnimator.SetFloat("Speed", 0f);
            return;
        }

        if (crouch && isGrounded)
        {
            horizontalMove = 0f;
            jorgeAnimator.SetFloat("Speed", 0f);
        }
        else
        {
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
            jorgeAnimator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        }

        if (Input.GetButtonDown("Jump") && !crouch)
        {
            jump = true;
            jorgeAnimator.SetBool("Jumping", true);
        }
    }

    public void StartGuitarSmash()
    {
        if (guitarHitBox == null)
        {
            Debug.LogError("ERROR: guitarHitBox is null!");
            return;
        }

        guitarHitBox.SetActive(true);
        Debug.Log("DEBUG 1: Hitbox activated");

        BoxCollider2D boxCollider = guitarHitBox.GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            Debug.LogError("ERROR: No BoxCollider2D found!");
            return;
        }

        float directionSign = Mathf.Sign(transform.localScale.x);
        Vector2 calculatedOffset = new Vector2(boxCollider.offset.x * directionSign, boxCollider.offset.y);
        Vector2 center = (Vector2)guitarHitBox.transform.position + calculatedOffset;
        Vector2 size = boxCollider.size;

        Collider2D[] hitObjects = Physics2D.OverlapBoxAll(center, size, 0f);

        bool hitEnemy = false;

        for (int i = 0; i < hitObjects.Length; i++)
        {
            if (hitObjects[i].CompareTag("Enemy"))
            {
                hitEnemy = true;

                Rigidbody2D enemyRb = hitObjects[i].GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    Vector2 knockbackForce = new Vector2(18f, 5f);

                    enemyRb.linearVelocity = Vector2.zero;

                    enemyRb.AddForce(knockbackForce, ForceMode2D.Impulse);

                    Debug.Log("Knockback applied to: " + hitObjects[i].name);
                }
            }
        }

        if (hitEnemy)
        {
            PlayerAudio pAudio = GetComponent<PlayerAudio>();
            if (pAudio != null)
            {
                pAudio.PlayGuitarSmashSound();
                Debug.Log("DEBUG SUCCESS: Sound triggered!");
            }
        }
        else
        {
            Debug.Log("DEBUG FAIL: No enemy tag found inside the box");
        }
    }

    public void EndGuitarSmash()
    {
        if (guitarHitBox != null)
        {
            guitarHitBox.SetActive(false);
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    public void OnLanding()
    {
        jorgeAnimator.SetBool("Jumping", false);
        jump = false;
    }

    void FixedUpdate()
    {
        jorgeController.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }
}