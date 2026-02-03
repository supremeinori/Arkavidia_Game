using UnityEngine;
using UnityEngine.InputSystem;

public class MovementAnimationController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference sprintAction;
    public InputActionReference jumpAction;

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.25f;
    public LayerMask groundLayer;

    [Header("Jump Settings")]
    public float jumpCooldown = 0.25f;

    bool isGrounded;
    float nextJumpTime;

    void Awake()
    {
        if (!animator)
            animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        if (moveAction) moveAction.action.Enable();
        if (sprintAction) sprintAction.action.Enable();
        if (jumpAction) jumpAction.action.Enable();
    }

    void OnDisable()
    {
        if (moveAction) moveAction.action.Disable();
        if (sprintAction) sprintAction.action.Disable();
        if (jumpAction) jumpAction.action.Disable();
    }

    void Update()
    {
        //-------------------
        // GROUND CHECK
        //-------------------
        if (groundCheckPoint)
        {
            isGrounded = Physics.CheckSphere(
                groundCheckPoint.position,
                groundCheckRadius,
                groundLayer);
        }
        else
        {
            isGrounded = true;
        }

        animator.SetBool("IsGrounded", isGrounded);

        //-------------------
        // MOVE INPUT
        //-------------------
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        bool moving = input.magnitude > 0.1f;

        bool sprinting =
            sprintAction &&
            sprintAction.action.IsPressed();

        animator.SetBool("IsWalk", moving && !sprinting);
        animator.SetBool("IsRun", moving && sprinting);

        //-------------------
        // FALLING
        //-------------------
        animator.SetBool("IsFalling", !isGrounded);

        //-------------------
        // JUMP / HIGH JUMP
        //-------------------
        bool jumpPressed =
            jumpAction &&
            jumpAction.action.WasPressedThisFrame();

        bool canJump =
            isGrounded &&
            Time.time >= nextJumpTime &&
            jumpPressed;

        if (canJump)
        {
            animator.ResetTrigger("Jump");
            animator.ResetTrigger("HighJump");

            if (sprinting)
                animator.SetTrigger("HighJump");
            else
                animator.SetTrigger("Jump");

            nextJumpTime = Time.time + jumpCooldown;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!groundCheckPoint) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(
            groundCheckPoint.position,
            groundCheckRadius);
    }
}
