using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Unity.Cinemachine.Samples
{
    public abstract class SimplePlayerControllerBase : MonoBehaviour
    {
        [Tooltip("Ground speed when walking")]
        public float Speed = 1f;

        [Tooltip("Ground speed when sprinting")]
        public float SprintSpeed = 4;

        [Tooltip("Initial vertical speed when jumping")]
        public float JumpSpeed = 4;

        [Tooltip("Initial vertical speed when sprint-jumping")]
        public float SprintJumpSpeed = 6;

        public Action PreUpdate;
        public Action<Vector3, float> PostUpdate;
        public Action StartJump;
        public Action EndJump;

        [Header("Events")]
        public UnityEvent Landed = new();

        [Header("Input Actions")]
        public InputActionReference MoveAction;
        public InputActionReference JumpAction;
        public InputActionReference SprintAction;

        public virtual void SetStrafeMode(bool b) { }
        public abstract bool IsMoving { get; }
    }

    public class SimplePlayerController : SimplePlayerControllerBase, ITeleportable
    {
        public float Damping = 0.5f;
        public bool Strafe = false;

        public enum ForwardModes { Camera, Player, World };
        public enum UpModes { Player, World };

        public ForwardModes InputForward = ForwardModes.Camera;
        public UpModes UpMode = UpModes.World;

        public Camera CameraOverride;

        public LayerMask GroundLayers = 1;

        public float Gravity = 10;

        const float kDelayBeforeInferringJump = 0.3f;

        float m_TimeLastGrounded;
        Vector3 m_CurrentVelocityXZ;
        Vector3 m_LastInput;
        float m_CurrentVelocityY;
        bool m_IsSprinting;
        bool m_IsJumping;

        CharacterController m_Controller;

        bool m_InTopHemisphere = true;
        float m_TimeInHemisphere = 100;
        Vector3 m_LastRawInput;
        Quaternion m_Upsidedown = Quaternion.AngleAxis(180, Vector3.left);

        public override bool IsMoving => m_LastInput.sqrMagnitude > 0.01f;
        public bool IsSprinting => m_IsSprinting;
        public bool IsJumping => m_IsJumping;

        public Camera Camera => CameraOverride == null ? Camera.main : CameraOverride;

        Vector3 UpDirection => UpMode == UpModes.World ? Vector3.up : transform.up;

        void Start() => TryGetComponent(out m_Controller);

        private void OnEnable()
        {
            if (MoveAction) MoveAction.action.Enable();
            if (JumpAction) JumpAction.action.Enable();
            if (SprintAction) SprintAction.action.Enable();

            m_CurrentVelocityY = 0;
            m_IsJumping = false;
            m_IsSprinting = false;
            m_TimeLastGrounded = Time.time;
        }

        private void OnDisable()
        {
            if (MoveAction) MoveAction.action.Disable();
            if (JumpAction) JumpAction.action.Disable();
            if (SprintAction) SprintAction.action.Disable();
        }

        void Update()
        {
            PreUpdate?.Invoke();

            bool justLanded = ProcessJump();

            Vector2 moveInput = MoveAction.action.ReadValue<Vector2>();
            var rawInput = new Vector3(moveInput.x, 0, moveInput.y);

            var inputFrame = GetInputFrame(Vector3.Dot(rawInput, m_LastRawInput) < 0.8f);
            m_LastRawInput = rawInput;

            m_LastInput = inputFrame * rawInput;
            if (m_LastInput.sqrMagnitude > 1)
                m_LastInput.Normalize();

            if (!m_IsJumping)
            {
                m_IsSprinting = SprintAction.action.IsPressed();

                var desiredVelocity = m_LastInput * (m_IsSprinting ? SprintSpeed : Speed);
                var damping = justLanded ? 0 : Damping;

                if (Vector3.Angle(m_CurrentVelocityXZ, desiredVelocity) < 100)
                    m_CurrentVelocityXZ = Vector3.Slerp(
                        m_CurrentVelocityXZ,
                        desiredVelocity,
                        Damper.Damp(1, damping, Time.deltaTime));
                else
                    m_CurrentVelocityXZ += Damper.Damp(
                        desiredVelocity - m_CurrentVelocityXZ,
                        damping,
                        Time.deltaTime);
            }

            ApplyMotion();

            if (!Strafe && m_CurrentVelocityXZ.sqrMagnitude > 0.001f)
            {
                var qA = transform.rotation;
                var qB = Quaternion.LookRotation(m_CurrentVelocityXZ, UpDirection);
                var damping = justLanded ? 0 : Damping;

                transform.rotation =
                    Quaternion.Slerp(qA, qB, Damper.Damp(1, damping, Time.deltaTime));
            }

            if (PostUpdate != null)
            {
                var vel = Quaternion.Inverse(transform.rotation) * m_CurrentVelocityXZ;
                vel.y = m_CurrentVelocityY;

                PostUpdate(vel, m_IsSprinting ? JumpSpeed / SprintJumpSpeed : 1);
            }
        }

        bool ProcessJump()
        {
            bool justLanded = false;

            bool grounded = IsGrounded();

            m_CurrentVelocityY -= Gravity * Time.deltaTime;

            if (!m_IsJumping)
            {
                if (grounded && JumpAction.action.WasPressedThisFrame())
                {
                    m_IsJumping = true;
                    m_CurrentVelocityY = m_IsSprinting ? SprintJumpSpeed : JumpSpeed;
                }

                if (!grounded && Time.time - m_TimeLastGrounded > kDelayBeforeInferringJump)
                    m_IsJumping = true;

                if (m_IsJumping)
                {
                    StartJump?.Invoke();
                    grounded = false;
                }
            }

            if (grounded)
            {
                m_TimeLastGrounded = Time.time;
                m_CurrentVelocityY = 0;

                if (m_IsJumping)
                {
                    EndJump?.Invoke();
                    m_IsJumping = false;
                    justLanded = true;
                    Landed.Invoke();
                }
            }

            return justLanded;
        }

        bool IsGrounded() =>
            GetDistanceFromGround(transform.position, UpDirection, 10) < 0.01f;

        void ApplyMotion()
        {
            if (m_Controller != null)
                m_Controller.Move((m_CurrentVelocityXZ + m_CurrentVelocityY * UpDirection) * Time.deltaTime);
            else
                transform.position +=
                    (m_CurrentVelocityXZ + m_CurrentVelocityY * UpDirection) * Time.deltaTime;
        }

        float GetDistanceFromGround(Vector3 pos, Vector3 up, float max)
        {
            float kExtraHeight = m_Controller == null ? 2 : 0;

            if (Physics.Raycast(
                    pos + up * kExtraHeight,
                    -up,
                    out var hit,
                    max + kExtraHeight,
                    GroundLayers,
                    QueryTriggerInteraction.Ignore))
                return hit.distance - kExtraHeight;

            return max + 1;
        }

        Quaternion GetInputFrame(bool inputDirectionChanged)
        {
            var frame = Quaternion.identity;

            switch (InputForward)
            {
                case ForwardModes.Camera: frame = Camera.transform.rotation; break;
                case ForwardModes.Player: return transform.rotation;
            }

            return frame;
        }

        public void Teleport(Vector3 newPos, Quaternion newRot)
        {
            if (m_Controller != null)
                m_Controller.enabled = false;

            transform.SetPositionAndRotation(newPos, newRot);

            if (m_Controller != null)
                m_Controller.enabled = true;
        }
    }
}
