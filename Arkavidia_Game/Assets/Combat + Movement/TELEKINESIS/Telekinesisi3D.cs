using UnityEngine;
using UnityEngine.InputSystem;

public class Telekinesis3D : MonoBehaviour
{
    [Header("Telekinesis Settings")]
    public float grabRange = 10f;
    public float holdDistance = 3f;
    public float moveForce = 10f;
    public float throwForce = 30f;

    [Header("Input Actions")]
    public InputActionReference grabAction;    // klik kanan
    public InputActionReference throwAction;   // klik kiri
    public InputActionReference aimAction;     // CTRL (aim)

    private Rigidbody heldObject;
    private TelekinesisObject tkObject;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void OnEnable()
    {
        if (grabAction) grabAction.action.Enable();
        if (throwAction) throwAction.action.Enable();
        if (aimAction) aimAction.action.Enable();
    }

    void OnDisable()
    {
        if (grabAction) grabAction.action.Disable();
        if (throwAction) throwAction.action.Disable();
        if (aimAction) aimAction.action.Disable();
    }

   void Update()
{
    bool grabHeld = grabAction.action.IsPressed();
    bool isAiming = grabHeld;

    // =====================
    // GRAB (RMB)
    // =====================
    if (grabAction.action.WasPressedThisFrame())
        GrabObject();

    // =====================
    // HOLD
    // =====================
    if (grabHeld && heldObject != null)
        HoldObject();

    // =====================
    // THROW (LMB)
    // =====================
    if (throwAction.action.WasPressedThisFrame() && heldObject != null)
        ThrowObject();

    // =====================
    // DROP (release RMB)
    // =====================
    if (!grabHeld && heldObject != null)
        DropObject();
}


    // ------------------------------------------------------

    void GrabObject()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out RaycastHit hit, grabRange))
        {
            if (hit.collider.CompareTag("Telekinesis"))
            {
                heldObject = hit.collider.GetComponent<Rigidbody>();
                tkObject = hit.collider.GetComponent<TelekinesisObject>();

                if (heldObject == null) return;

                heldObject.useGravity = false;
                heldObject.linearDamping = 10f;

                if (tkObject != null)
                    tkObject.SetThrown(false);
            }
        }
    }

    void HoldObject()
    {
        Vector3 targetPos =
            cam.transform.position +
            cam.transform.forward * holdDistance;

        Vector3 direction = targetPos - heldObject.position;

        heldObject.linearVelocity = direction * moveForce;
    }

    void DropObject()
    {
        if (heldObject == null) return;

        heldObject.useGravity = true;
        heldObject.linearDamping = 1f;

        heldObject = null;
        tkObject = null;
    }

    void ThrowObject()
    {
        if (heldObject == null) return;

        heldObject.useGravity = true;
        heldObject.linearDamping = 0;

        heldObject.linearVelocity = Vector3.zero;
        heldObject.angularVelocity = Vector3.zero;

        heldObject.AddForce(
            (cam.transform.forward + Vector3.up * 0.1f) * throwForce,
            ForceMode.VelocityChange
        );

        if (tkObject != null)
            tkObject.SetThrown(true);

        heldObject = null;
        tkObject = null;
    }
}
