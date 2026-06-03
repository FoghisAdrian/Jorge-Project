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