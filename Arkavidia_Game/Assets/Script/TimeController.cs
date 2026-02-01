using UnityEngine;

public class TimeController : MonoBehaviour
{
    public Transform player;

    // waktu sekarang = posisi Z player
    public float CurrentTimeZ { get; private set; }

    void Update()
    {
        if (!player) return;
        CurrentTimeZ = player.position.z;
    }
}
