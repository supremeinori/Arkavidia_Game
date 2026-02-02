using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator animator;

    public InputActionReference moveAction;
    public InputActionReference sprintAction;

    private void OnEnable()
    {
        moveAction.action.Enable();
        sprintAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        sprintAction.action.Disable();
    }

void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // baca input
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        // === WALK CHECK ===
        bool isWalking = input.magnitude > 0.1f;

        // === RUN CHECK ===
        bool isRunning =
            isWalking &&
            sprintAction.action.IsPressed();
        
        // === KIRIM KE ANIMATOR ===
        animator.SetBool("Iswalk", isWalking);
        animator.SetBool("Isrun", isRunning);
    }
}

