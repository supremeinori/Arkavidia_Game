using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.Cinemachine.Samples
{
    public class CursorLock2 : MonoBehaviour, Unity.Cinemachine.IInputAxisOwner
    {
        public InputAxis CursorLock = InputAxis.DefaultMomentary;

        public UnityEvent OnCursorLocked = new();
        public UnityEvent OnCursorUnlocked = new();

        bool m_IsTriggered;
        bool allowToggle = true;

        public void GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
        {
            axes.Add(new()
            {
                DrivenAxis = () => ref CursorLock,
                Name = "CursorLock",
                Hint = IInputAxisOwner.AxisDescriptor.Hints.X
            });
        }

        void OnValidate() => CursorLock.Validate();

        void Start()
        {
            // Gameplay start = locked
            LockCursor();
        }

        void Update()
        {
            if (!allowToggle)
                return;

            if (CursorLock.Value == 0)
                m_IsTriggered = false;
            else if (!m_IsTriggered)
            {
                m_IsTriggered = true;

                if (Cursor.lockState == CursorLockMode.None)
                    LockCursor();
                else
                    UnlockCursor();
            }
        }

        // ==========================
        // CALLED BY GAME MANAGER
        // ==========================

        public void ForceLock()
        {
            allowToggle = true;
            LockCursor();
        }

        public void ForceUnlock()
        {
            allowToggle = false;
            UnlockCursor();
        }

        public void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            OnCursorLocked.Invoke();
        }

        public void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            OnCursorUnlocked.Invoke();
        }
    }
}
