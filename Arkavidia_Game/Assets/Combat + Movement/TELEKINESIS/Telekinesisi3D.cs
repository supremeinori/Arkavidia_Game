using UnityEngine;

public class Telekinesis3D : MonoBehaviour
{
    public float grabRange = 10f;
    public float holdDistance = 3f;
    public float moveForce = 10f;
    public float throwForce = 30f;

    private Rigidbody heldObject;
    private Camera cam;
    private TelekinesisObject tkObject;


    void Start()
    {
        cam = Camera.main;
    }

void Update()
{
    if (Input.GetMouseButtonDown(1))
        GrabObject();

    if (Input.GetMouseButton(1) && heldObject != null)
        HoldObject();

    if (Input.GetMouseButtonDown(0) && heldObject != null)
        ThrowObject();

    if (Input.GetMouseButtonUp(1))
        DropObject();
}


void GrabObject()
{
    Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
    if (Physics.Raycast(ray, out RaycastHit hit, grabRange))
    {
        if (hit.collider.CompareTag("Telekinesis"))
        {
            heldObject = hit.collider.GetComponent<Rigidbody>();
            tkObject = hit.collider.GetComponent<TelekinesisObject>();

            heldObject.useGravity = false;
            heldObject.linearDamping = 10;

            tkObject.SetThrown(false);
        }
    }
}


    void HoldObject()
    {
        Vector3 targetPos = cam.transform.position + cam.transform.forward * holdDistance;
        Vector3 direction = targetPos - heldObject.position;
        heldObject.linearVelocity = direction * moveForce;
    }

    void DropObject()
    {
    if (heldObject == null) return;

    heldObject.useGravity = true;
    heldObject.linearDamping = 1;
    heldObject = null;
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

       // ✅ TAMBAHAN (WAJIB)
    if (tkObject != null)
        tkObject.SetThrown(true);

    heldObject = null;
    tkObject = null;
}


}
