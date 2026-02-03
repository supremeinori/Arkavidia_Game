using UnityEngine;

public class CombatController : MonoBehaviour
{
    public SwordHandler swordHandler;
    public Animator animator;

    bool isAttacking;

    void Update()
    {
        HandleEquip();
        HandleAttack();
    }

    void HandleEquip()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (swordHandler.isEquipped)
                animator.SetTrigger("Sheath");
            else
                animator.SetTrigger("Equip");
        }
    }

    void HandleAttack()
    {
        if (!swordHandler.isEquipped) return;
        if (isAttacking) return;

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("LightAttack");
            isAttacking = true;
        }
        else if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("HeavyAttack");
            isAttacking = true;
        }
    }

    // dipanggil lewat Animation Event
    public void EndAttack()
    {
        isAttacking = false;
    }

    // dipanggil lewat Animation Event
    public void EquipSword()
    {
        swordHandler.Equip();
    }

    public void UnequipSword()
    {
        swordHandler.Unequip();
    }
}
