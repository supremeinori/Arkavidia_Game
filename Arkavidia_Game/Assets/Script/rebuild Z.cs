using UnityEditor.Rendering;
using UnityEngine;

public class TimeRebuilZ : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform timeSwitchPoint;
    public GameObject intactReference;

    [Header("Pieces")]
    public Rigidbody[] pieces;

    [Header("Rebuild Settings")]
    public float rebuildSpeed = 3f;

    Vector3[] initialLocalPos;
    Quaternion[] initialLocalRot;

    void Awake()
    {
        // 🔥 AUTO ATTACH SEMUA PIECES
        pieces = GetComponentsInChildren<Rigidbody>();
    }

    void Start()
    {
        initialLocalPos = new Vector3[pieces.Length];
        initialLocalRot = new Quaternion[pieces.Length];

        for (int i = 0; i < pieces.Length; i++)
        {
            initialLocalPos[i] = pieces[i].transform.localPosition;
            initialLocalRot[i] = pieces[i].transform.localRotation;
        }

        // Awal game = PAST
        if (intactReference)
            intactReference.SetActive(false);
    }

    void Update()
    {
        if (!player || !timeSwitchPoint) return;

        bool isPresent = player.transform.position.z > timeSwitchPoint.position.z;

        if (intactReference)
            intactReference.SetActive(!isPresent);

        for (int i = 0; i < pieces.Length; i++)
        {
            if (isPresent)
            {
                pieces[i].isKinematic = true;

                pieces[i].transform.localPosition =
                    Vector3.Lerp(
                        pieces[i].transform.localPosition,
                        initialLocalPos[i],
                        Time.deltaTime * rebuildSpeed
                    );

                pieces[i].transform.localRotation =
                    Quaternion.Lerp(
                        pieces[i].transform.localRotation,
                        initialLocalRot[i],
                        Time.deltaTime * rebuildSpeed
                    );
            }
            else
            {
                pieces[i].isKinematic = false;
            }
        }
    }
}
