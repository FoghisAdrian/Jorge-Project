using UnityEngine;
using UnityEngine.Events;

public class JorgeController : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;
    [Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;
    [SerializeField] private bool m_AirControl = false;
    [SerializeField] private LayerMask m_WhatIsGround;
    [SerializeField] private Transform m_GroundCheck;
    [SerializeField] private Transform m_CeilingCheck;
    [SerializeField] private Collider2D m_CrouchDisableCollider;

    const float k_GroundedRadius = .2f;
    public bool m_Grounded;
    const float k_CeilingRadius = .2f;
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;
    private Vector3 m_Velocity = Vector3.zero;

    private Collider2D m_MainCollider;
    private Vector2 m_OriginalSize;
    private Vector2 m_OriginalOffset;

    [Header("Events")]
    [Space]
    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_MainCollider = GetComponent<Collider2D>();

        if (m_MainCollider is CapsuleCollider2D)
        {
            m_OriginalSize = ((CapsuleCollider2D)m_MainCollider).size;
            m_OriginalOffset = ((CapsuleCollider2D)m_MainCollider).offset;
        }
        else if (m_MainCollider is BoxCollider2D)
        {
            m_OriginalSize = ((BoxCollider2D)m_MainCollider).size;
            m_OriginalOffset = ((BoxCollider2D)m_MainCollider).offset;
        }

        if (OnLandEvent == null) OnLandEvent = new UnityEvent();
        if (OnCrouchEvent == null) OnCrouchEvent = new BoolEvent();
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
    }

    public void Move(float move, bool crouch, bool jump)
    {
        if (!crouch)
        {
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }

        if (m_Grounded || m_AirControl)
        {
            if (crouch)
            {
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }

                m_Rigidbody2D.linearVelocity = new Vector2(0f, m_Rigidbody2D.linearVelocity.y);
                m_Velocity = Vector3.zero; 
                move = 0f;

                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }
            else
            {
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = true;

                if (m_wasCrouching)
                {
                    m_wasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                    m_Velocity = Vector3.zero; 
                }
            }

            if (!crouch)
            {
                Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.linearVelocity.y);
                m_Rigidbody2D.linearVelocity = Vector3.SmoothDamp(m_Rigidbody2D.linearVelocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
            }

            if (move > 0 && !m_FacingRight) Flip();
            else if (move < 0 && m_FacingRight) Flip();
        }

        if (m_Grounded && jump)
        {
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));

            PlayerAudio pAudio = GetComponent<PlayerAudio>();
            if (pAudio != null)
            {
                pAudio.PlayJumpSound();
            }
        }
    }

    private void AdjustColliderHeight(bool isCrouching)
    {
        if (m_MainCollider == null) return;

        if (isCrouching)
        {
            if (m_MainCollider is CapsuleCollider2D capsule)
            {
                capsule.size = new Vector2(m_OriginalSize.x, m_OriginalSize.y * 0.55f);
                capsule.offset = new Vector2(m_OriginalOffset.x, m_OriginalOffset.y - (m_OriginalSize.y * 0.22f));
            }
            else if (m_MainCollider is BoxCollider2D box)
            {
                box.size = new Vector2(m_OriginalSize.x, m_OriginalSize.y * 0.55f);
                box.offset = new Vector2(m_OriginalOffset.x, m_OriginalOffset.y - (m_OriginalSize.y * 0.22f));
            }
        }
        else
        {
            if (m_MainCollider is CapsuleCollider2D capsule)
            {
                capsule.size = m_OriginalSize;
                capsule.offset = m_OriginalOffset;
            }
            else if (m_MainCollider is BoxCollider2D box)
            {
                box.size = m_OriginalSize;
                box.offset = m_OriginalOffset;
            }
        }
    }

    private void Flip()
    {
        m_FacingRight = !m_FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}