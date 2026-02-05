using UnityEngine;

public class EnemyVisualAnimator : MonoBehaviour
{
    public Animator animator;

    void Awake()
    {
        if (!animator)
            animator = GetComponent<Animator>();
    }

    public void SetIdle()
    {
        animator.SetBool("isWalking", false);
    }

    public void SetRun()
    {
        animator.SetBool("isWalking", true);
    }

    public void TriggerAttack()
    {
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");
    }
}
