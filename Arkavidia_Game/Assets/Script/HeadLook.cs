using UnityEngine;

public class HeadLook : MonoBehaviour
{
    public Transform cameraTransform;
    public Transform headBone;

    public float maxYaw = 50f;
    public float maxPitch = 25f;
    public float followSpeed = 5f;

    private Quaternion animRotation;

    void LateUpdate()
    {
        if (!headBone || !cameraTransform) return;

        // 🔥 ambil pose animasi SAAT INI (AMAN)
        animRotation = headBone.localRotation;

        Vector3 localDir =
            headBone.parent.InverseTransformDirection(cameraTransform.forward);

        Quaternion lookRot = Quaternion.LookRotation(localDir);

        Vector3 euler = lookRot.eulerAngles;
        euler.x = Clamp(euler.x, -maxPitch, maxPitch);
        euler.y = Clamp(euler.y, -maxYaw, maxYaw);
        euler.z = 0f;

        Quaternion offset = Quaternion.Euler(euler);

        // smooth offset tanpa akumulasi
        headBone.localRotation = Quaternion.Slerp(
            animRotation,
            animRotation * offset,
            Time.deltaTime * followSpeed
        );
    }

    float Clamp(float angle, float min, float max)
    {
        if (angle > 180) angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}
