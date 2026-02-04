using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;

    [Header("Jump & Gravity")]
    public float gravity = -20f;
    public float jumpHeight = 1.5f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        Setup();
    }

    void Update()
    {
        CheckGround();
        Move();
        Jump();
        ApplyGravity();
    }

    // =========================
    // SETUP
    // =========================
    void Setup()
    {
        controller = GetComponent<CharacterController>();
    }


    void CheckGround()
    {
        isGrounded = controller.isGrounded;

        // Menjaga player tetap menempel tanah
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }


    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float speed = Input.GetKey(KeyCode.LeftShift)
            ? sprintSpeed
            : walkSpeed;

        Vector3 move =
            transform.right * x +
            transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);
    }


    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Debug.Log("Jump!");
            velocity.y = CalculateJumpVelocity();
        }
    }

    float CalculateJumpVelocity()
    {
        return Mathf.Sqrt(jumpHeight * -2f * gravity);
    }


    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
