using System.Collections.Generic;
using UnityEngine;

namespace Unity.Cinemachine.Samples
{
    public class SimplePlayerAimController : MonoBehaviour, Unity.Cinemachine.IInputAxisOwner
    {
        public enum CouplingMode { Coupled, CoupledWhenMoving, Decoupled }

        [Tooltip("How the player's rotation is coupled to the camera's rotation.")]
        public CouplingMode PlayerRotation = CouplingMode.CoupledWhenMoving;

        [Tooltip("How fast the player rotates to face the camera direction when the player starts moving.")]
        public float RotationDamping = 0.2f;

        [Tooltip("Horizontal Rotation.  Value is in degrees, with 0 being centered.")]
        public InputAxis HorizontalLook = new()
        {
            Range = new Vector2(-180, 180),
            Wrap = true,
            Recentering = InputAxis.RecenteringSettings.Default
        };

        [Tooltip("Vertical Rotation.  Value is in degrees, with 0 being centered.")]
        public InputAxis VerticalLook = new()
        {
            Range = new Vector2(-70, 70),
            Recentering = InputAxis.RecenteringSettings.Default
        };

        SimplePlayerControllerBase m_Controller;
        Transform m_ControllerTransform;
        Quaternion m_DesiredWorldRotation;

        // =====================================================
        // INPUT AXIS OWNER
        // =====================================================
        void IInputAxisOwner.GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
        {
            axes.Add(new()
            {
                DrivenAxis = () => ref HorizontalLook,
                Name = "Horizontal Look",
                Hint = IInputAxisOwner.AxisDescriptor.Hints.X
            });

            axes.Add(new()
            {
                DrivenAxis = () => ref VerticalLook,
                Name = "Vertical Look",
                Hint = IInputAxisOwner.AxisDescriptor.Hints.Y
            });
        }

        void OnValidate()
        {
            HorizontalLook.Validate();

            VerticalLook.Range.x = Mathf.Clamp(VerticalLook.Range.x, -90, 90);
            VerticalLook.Range.y = Mathf.Clamp(VerticalLook.Range.y, -90, 90);
            VerticalLook.Validate();
        }

        // =====================================================
        // UNITY EVENTS
        // =====================================================
        void OnEnable()
        {
            m_Controller = GetComponentInParent<SimplePlayerControllerBase>();

            if (m_Controller == null)
            {
                Debug.LogError("SimplePlayerController not found on parent object");
                return;
            }

            m_Controller.PreUpdate -= UpdatePlayerRotation;
            m_Controller.PreUpdate += UpdatePlayerRotation;

            m_Controller.PostUpdate -= PostUpdate;
            m_Controller.PostUpdate += PostUpdate;

            m_ControllerTransform = m_Controller.transform;
        }

        void OnDisable()
        {
            if (m_Controller != null)
            {
                m_Controller.PreUpdate -= UpdatePlayerRotation;
                m_Controller.PostUpdate -= PostUpdate;
            }

            m_ControllerTransform = null;
        }

        // =====================================================
        // CAMERA / AIM CORE
        // =====================================================
        void UpdatePlayerRotation()
        {
            var t = transform;

            t.localRotation = Quaternion.Euler(
                VerticalLook.Value,
                HorizontalLook.Value,
                0);

            m_DesiredWorldRotation = t.rotation;

            switch (PlayerRotation)
            {
                case CouplingMode.Coupled:
                {
                    m_Controller.SetStrafeMode(true);
                    RecenterPlayer();
                    break;
                }

                case CouplingMode.CoupledWhenMoving:
                {
                    m_Controller.SetStrafeMode(true);

                    if (m_Controller.IsMoving)
                        RecenterPlayer(RotationDamping);

                    break;
                }

                case CouplingMode.Decoupled:
                {
                    m_Controller.SetStrafeMode(false);
                    break;
                }
            }

            VerticalLook.UpdateRecentering(Time.deltaTime, VerticalLook.TrackValueChange());
            HorizontalLook.UpdateRecentering(Time.deltaTime, HorizontalLook.TrackValueChange());
        }

        void PostUpdate(Vector3 vel, float speed)
        {
            if (PlayerRotation == CouplingMode.Decoupled && m_ControllerTransform != null)
            {
                transform.rotation = m_DesiredWorldRotation;

                var delta =
                    (Quaternion.Inverse(m_ControllerTransform.rotation)
                        * m_DesiredWorldRotation).eulerAngles;

                VerticalLook.Value = NormalizeAngle(delta.x);
                HorizontalLook.Value = NormalizeAngle(delta.y);
            }
        }

        // =====================================================
        // HELPERS
        // =====================================================
        public void RecenterPlayer(float damping = 0)
        {
            if (m_ControllerTransform == null)
                return;

            var rot = transform.localRotation.eulerAngles;
            rot.y = NormalizeAngle(rot.y);

            float delta = rot.y;
            delta = Damper.Damp(delta, damping, Time.deltaTime);

            m_ControllerTransform.rotation =
                Quaternion.AngleAxis(delta, m_ControllerTransform.up)
                * m_ControllerTransform.rotation;

            HorizontalLook.Value -= delta;

            rot.y -= delta;
            transform.localRotation = Quaternion.Euler(rot);
        }

        float NormalizeAngle(float angle)
        {
            while (angle > 180) angle -= 360;
            while (angle < -180) angle += 360;
            return angle;
        }
    }
}
