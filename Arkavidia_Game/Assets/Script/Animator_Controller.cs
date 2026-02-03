using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator animator;

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference sprintAction;
    public InputActionReference jumpAction;

    [Header("Jump Settings")]
    public float jumpRepeatDelay = 0.35f;

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.25f;
    public LayerMask groundLayer;

    float nextJumpTime;
    bool isGrounded;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (moveAction) moveAction.action.Enable();
        if (sprintAction) sprintAction.action.Enable();
        if (jumpAction) jumpAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction) moveAction.action.Disable();
        if (sprintAction) sprintAction.action.Disable();
        if (jumpAction) jumpAction.action.Disable();
    }

    void Update()
    {
        // =====================
        // GROUND CHECK
        // =====================
        isGrounded = Physics.CheckSphere(
            groundCheckPoint.position,
            groundCheckRadius,
            groundLayer);

        animator.SetBool("IsGrounded", isGrounded);

        // =====================
        // MOVE
        // =====================
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        bool isWalking = input.magnitude > 0.1f;

        bool isRunning =
            isWalking &&
            sprintAction.action.IsPressed();

        // =====================
        // JUMP INPUT (ONLY WHEN GROUNDED)
        // =====================
        bool jumpHeld = jumpAction.action.IsPressed();
        bool sprintHeld = sprintAction.action.IsPressed();

        bool canTriggerJump =
            isGrounded &&
            jumpHeld &&
            Time.time >= nextJumpTime;

        bool normalJump = canTriggerJump && !sprintHeld;
        bool longJump = canTriggerJump && sprintHeld;

        if (canTriggerJump)
            nextJumpTime = Time.time + jumpRepeatDelay;

        // =====================
        // SEND TO ANIMATOR
        // =====================
        animator.SetBool("Iswalk", isWalking);
        animator.SetBool("Isrun", isRunning);

        animator.SetBool("IsJump", normalJump);
        animator.SetBool("IsLongJump", longJump);
    }

    // Optional: visualize ground check
    void OnDrawGizmosSelected()
    {
        if (!groundCheckPoint) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }
}
