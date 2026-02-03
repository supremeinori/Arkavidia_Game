using UnityEngine;

public class SwordHandler : MonoBehaviour
{
    public Transform backSocket;
    public Transform handSocket;
    public GameObject sword;

    public bool isEquipped { get; private set; }

    void Start()
    {
        if (sword == null)
        {
            Debug.LogError("Sword belum di-assign!");
            return;
        }

        AttachToBack();
    }

    public void Equip()
    {
        sword.transform.SetParent(handSocket);
        ResetTransform();
        isEquipped = true;
    }

    public void Unequip()
    {
        AttachToBack();
    }

    void AttachToBack()
    {
        sword.transform.SetParent(backSocket);
        ResetTransform();
        isEquipped = false;
    }

    void ResetTransform()
    {
        sword.transform.localPosition = Vector3.zero;
        sword.transform.localRotation = Quaternion.identity;
    }
}
