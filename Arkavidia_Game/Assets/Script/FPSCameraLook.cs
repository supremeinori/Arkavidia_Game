using UnityEngine;

public class FPSCameraLook : MonoBehaviour
{
    public float mouseSensitivity = 150f;
    public Transform playerBody; // Player root

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // pitch (atas-bawah)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // yaw (kiri-kanan)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
