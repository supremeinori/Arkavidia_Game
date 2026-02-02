using UnityEngine;

public class TimeCollapseObject : MonoBehaviour
{
    public TimeController timeController;
    public Transform timeSwitchPoint;

    [Header("States")]
    public GameObject intactObject;   // Tangga utuh (PRESENT)
    public Rigidbody[] pieces;        // Pecahan tangga (PAST)

    bool isCollapsed = false;

    void Start()
    {
        // Start di PRESENT
        intactObject.SetActive(true);

        foreach (var rb in pieces)
        {
            rb.gameObject.SetActive(false);
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    void Update()
    {
        if (!timeController || !timeSwitchPoint) return;

        bool shouldBePast =
            timeController.CurrentTimeZ < timeSwitchPoint.position.z;

        if (shouldBePast && !isCollapsed)
            Collapse();

        if (!shouldBePast && isCollapsed)
            Restore();
    }

    void Collapse()
    {
        isCollapsed = true;

        intactObject.SetActive(false);

        foreach (var rb in pieces)
        {
            rb.gameObject.SetActive(true);
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    void Restore()
    {
        isCollapsed = false;

        intactObject.SetActive(true);

        foreach (var rb in pieces)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.gameObject.SetActive(false);
        }
    }
}
