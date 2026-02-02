using UnityEditor.Rendering;
using UnityEngine;

public class TimeRebuilZ : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform timeSwitchPoint;      // Titik switch waktu
    public GameObject intactReference;     // Tangga utuh (visual helper)

    [Header("Pieces")]
    public Rigidbody[] pieces;

    [Header("Rebuild Settings")]
    public float rebuildSpeed = 3f;

    Vector3[] initialLocalPos;
    Quaternion[] initialLocalRot;

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

        // 🔥 HANDLE TANGGA UTUH
        if (intactReference)
            intactReference.SetActive(!isPresent); 
        // present → OFF, past → ON (kalau mau ditampilkan)

        for (int i = 0; i < pieces.Length; i++)
        {
            if (isPresent)
            {
                // 🛠️ REBUILD
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
                // 💥 RUNTUH
                pieces[i].isKinematic = false;
            }
        }
    }
}
