using UnityEngine;
using UnityEngine.InputSystem;

public class JorgeMovement : MonoBehaviour
{
    public JorgeController jorgeController;
    public float horizontalMove = 0f;
    public float runSpeed = 40f;
    public bool jump = false;
    public Animator jorgeAnimator;
    public bool isAttacking = false;

    void Update()
    {
        bool isGrounded = jorgeController.m_Grounded;

        if (!isAttacking && isGrounded && (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Fire1")))
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

        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        jorgeAnimator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if(Input.GetButtonDown("Jump"))
        {
            jump = true;
            jorgeAnimator.SetBool("Jumping", true);
        }

        
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    public void OnLanding ()
    {
        jorgeAnimator.SetBool("Jumping", false);
        jump = false;
    }

    void FixedUpdate()
    {
        jorgeController.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        jump = false;   
    }
}
