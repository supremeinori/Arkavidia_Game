using UnityEngine;

public class PauseUIRegister : MonoBehaviour
{
    void Awake()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.pausePanel = gameObject;
            gameObject.SetActive(false);
        }
    }
}
