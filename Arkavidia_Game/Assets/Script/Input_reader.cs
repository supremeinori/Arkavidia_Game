// This first example shows how to move using Input System Package (New)

using UnityEngine;
using UnityEngine.InputSystem;

public class Example : MonoBehaviour
{
    public float playerSpeed = 5.0f;
    public float playerSprint = 10.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;
    private float currentSpeed = 0.0f;

    public CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference jumpAction;
    public InputActionReference sprintAction;

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer)
        {
            // Slight downward velocity to keep grounded stable
            if (playerVelocity.y < -2f)
                playerVelocity.y = -2f;
        }

        // Read input
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = Vector3.ClampMagnitude(move, 1f);
	// Move condition
        if (move != Vector3.zero)
            transform.forward = move;

	//check sprint
	 bool isSprinting = sprintAction.action.IsPressed();
	float currentSpeed = isSprinting ? playerSprint : playerSpeed;

        // Jump using WasPressedThisFrame()
        if (groundedPlayer && jumpAction.action.WasPressedThisFrame())
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
        }

        // Apply gravity
        playerVelocity.y += gravityValue * Time.deltaTime;

        // Move
         Vector3 finalMove =
        move * currentSpeed +
        Vector3.up * playerVelocity.y;

    controller.Move(finalMove * Time.deltaTime);
    }
}
