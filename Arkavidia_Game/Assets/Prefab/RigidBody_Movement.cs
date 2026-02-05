using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyPlayerInputSystem : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public float airControlMultiplier = 0.5f;

    [Header("Jump")]
    public float jumpForce = 7f;
    public float highJumpMultiplier = 1.4f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.25f;
    public LayerMask groundMask;

    Rigidbody rb;
    bool isGrounded;

    Vector2 moveInput;
    bool sprintHeld;
    bool jumpPressed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // ===== INPUT SYSTEM CALLBACKS =====
    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext ctx)
    {
        sprintHeld = ctx.ReadValueAsButton();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            jumpPressed = true;
    }

    void Update()
    {
        CheckGround();
    }

    void FixedUpdate()
    {
        Move();

        if (jumpPressed && isGrounded)
        {
            Jump();
            jumpPressed = false;
        }
    }

    void Move()
    {
        Vector3 moveDir =
            transform.right * moveInput.x +
            transform.forward * moveInput.y;

        float speed = sprintHeld ? sprintSpeed : walkSpeed;

        if (!isGrounded)
            speed *= airControlMultiplier;

        Vector3 targetVelocity = moveDir.normalized * speed;

        Vector3 currentVel =
            new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        Vector3 velocityChange = targetVelocity - currentVel;

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        float finalJumpForce = jumpForce;

        if (sprintHeld)
            finalJumpForce *= highJumpMultiplier;

        rb.AddForce(Vector3.up * finalJumpForce, ForceMode.Impulse);
    }

    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundRadius,
            groundMask
        );
    }
}
