using UnityEngine;

public class TimeReactiveObject : MonoBehaviour
{
    public TimeController timeController;
    public Transform timeSwitchPoint;

    [Header("States")]
    public GameObject presentState;
    public GameObject pastState;

    void Update()
    {
        if (!timeController || !timeSwitchPoint) return;

        bool isPast = timeController.CurrentTimeZ < timeSwitchPoint.position.z;

        presentState.SetActive(!isPast);
        pastState.SetActive(isPast);
    }
}
