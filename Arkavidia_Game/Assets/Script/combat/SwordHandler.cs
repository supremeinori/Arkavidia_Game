using UnityEngine;

public class SwordHandler : MonoBehaviour
{
    public Transform backSocket;
    public Transform handSocket;
    public GameObject sword;

    public bool isEquipped { get; private set; }

    void Start()
    {
        AttachToBack();
    }

    public void Equip()
    {
        sword.transform.SetParent(handSocket);
        sword.transform.localPosition = Vector3.zero;
        sword.transform.localRotation = Quaternion.identity;
        isEquipped = true;
    }

    public void Unequip()
    {
        AttachToBack();
    }

    void AttachToBack()
    {
        sword.transform.SetParent(backSocket);
        sword.transform.localPosition = Vector3.zero;
        sword.transform.localRotation = Quaternion.identity;
        isEquipped = false;
    }
}
