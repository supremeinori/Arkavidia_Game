using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.Cinemachine.Samples
{
    /// <summary>
    /// This is the base class for SimplePlayerController and SimplePlayerController2D.
    /// You can also use it as a base class for your custom controllers.
    /// It provides the following:
    ///
    /// **Services:**
    ///
    /// - 2D motion axes (MoveX and MoveZ)
    /// - Jump button
    /// - Sprint button
    /// - API for strafe mode
    ///
    /// **Actions:**
    ///
    /// - PreUpdate - invoked at the beginning of Update()
    /// - PostUpdate - invoked at the end of Update()
    /// - StartJump - invoked when the player starts jumping
    /// - EndJump - invoked when the player stops jumping
    ///
    /// **Events:**
    ///
    /// - Landed - invoked when the player lands on the ground
    /// </summary>
    public abstract class SimplePlayerControllerBase : MonoBehaviour, Unity.Cinemachine.IInputAxisOwner
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

        [Header("Input Axes")]
        [Tooltip("X Axis movement. Value is -1..1. Controls the sideways movement")]
        public InputAxis MoveX = InputAxis.DefaultMomentary;

        [Tooltip("Z Axis movement. Value is -1..1. Controls the forward movement")]
        public InputAxis MoveZ = InputAxis.DefaultMomentary;

        [Tooltip("Jump movement. Value is 0 or 1. Controls the vertical movement")]
        public InputAxis Jump = InputAxis.DefaultMomentary;

        [Tooltip("Sprint movement. Value is 0 or 1. If 1, then is sprinting")]
        public InputAxis Sprint = InputAxis.DefaultMomentary;

        [Header("Events")]
        [Tooltip("This event is sent when the player lands after a jump.")]
        public UnityEvent Landed = new();

        void Unity.Cinemachine.IInputAxisOwner.GetInputAxes(
            List<Unity.Cinemachine.IInputAxisOwner.AxisDescriptor> axes)
        {
            axes.Add(new()
            {
                DrivenAxis = () => ref MoveX,
                Name = "Move X",
                Hint = Unity.Cinemachine.IInputAxisOwner.AxisDescriptor.Hints.X
            });

            axes.Add(new()
            {
                DrivenAxis = () => ref MoveZ,
                Name = "Move Z",
                Hint = Unity.Cinemachine.IInputAxisOwner.AxisDescriptor.Hints.Y
            });

            axes.Add(new()
            {
                DrivenAxis = () => ref Jump,
                Name = "Jump"
            });

            axes.Add(new()
            {
                DrivenAxis = () => ref Sprint,
                Name = "Sprint"
            });
        }

        protected virtual void OnValidate()
        {
            MoveX.Validate();
            MoveZ.Validate();
            Jump.Validate();
            Sprint.Validate();
        }

        public virtual void SetStrafeMode(bool b) { }

        public abstract bool IsMoving { get; }
    }

    /// <summary>
    /// Building on top of SimplePlayerControllerBase, this is the 3D character controller.
    /// </summary>
    public class SimplePlayerController : SimplePlayerControllerBase, ITeleportable
    {
        [Tooltip("Transition duration (in seconds) when the player changes velocity or rotation.")]
        public float Damping = 0.5f;

        [Tooltip("Makes the player strafe when moving sideways, otherwise it turns to face the direction of motion.")]
        public bool Strafe = false;

        public enum ForwardModes { Camera, Player, World }
        public enum UpModes { Player, World }

        [Tooltip(
            "Reference frame for the input controls:\n" +
            "<b>Camera</b>: Input forward is camera forward direction.\n" +
            "<b>Player</b>: Input forward is Player's forward direction.\n" +
            "<b>World</b>: Input forward is World forward direction."
        )]
        public ForwardModes InputForward = ForwardModes.Camera;

        [Tooltip(
            "Up direction for computing motion:\n" +
            "<b>Player</b>: Move in the Player's local XZ plane.\n" +
            "<b>World</b>: Move in global XZ plane."
        )]
        public UpModes UpMode = UpModes.World;

        [Tooltip(
            "If non-null, take the input frame from this camera instead of Camera.main. Useful for split-screen games."
        )]
        public Camera CameraOverride;

        [Tooltip("Layers to include in ground detection via Raycasts.")]
        public LayerMask GroundLayers = 1;

        [Tooltip("Force of gravity in the down direction (m/s^2)")]
        public float Gravity = 10;

        const float kDelayBeforeInferringJump = 0.3f;

        float m_TimeLastGrounded = 0;
        Vector3 m_CurrentVelocityXZ;
        Vector3 m_LastInput;
        float m_CurrentVelocityY;
        bool m_IsSprinting;
        bool m_IsJumping;

        UnityEngine.CharacterController m_Controller;

        bool m_InTopHemisphere = true;
        float m_TimeInHemisphere = 100;
        Vector3 m_LastRawInput;

        Quaternion m_Upsidedown = Quaternion.AngleAxis(180, Vector3.left);

        public override void SetStrafeMode(bool b) => Strafe = b;

        public override bool IsMoving => m_LastInput.sqrMagnitude > 0.01f;

        public bool IsSprinting => m_IsSprinting;
        public bool IsJumping => m_IsJumping;

        public Camera Camera => CameraOverride == null ? Camera.main : CameraOverride;

        public bool IsGrounded() =>
            GetDistanceFromGround(transform.position, UpDirection, 10) < 0.01f;

        void Start() => TryGetComponent(out m_Controller);

        private void OnEnable()
        {
            m_CurrentVelocityY = 0;
            m_IsSprinting = false;
            m_IsJumping = false;
            m_TimeLastGrounded = Time.time;
        }

        void Update()
        {
            PreUpdate?.Invoke();

            bool justLanded = ProcessJump();

            var rawInput = new Vector3(MoveX.Value, 0, MoveZ.Value);
            var inputFrame = GetInputFrame(
                Vector3.Dot(rawInput, m_LastRawInput) < 0.8f);

            m_LastRawInput = rawInput;

            m_LastInput = inputFrame * rawInput;
            if (m_LastInput.sqrMagnitude > 1)
                m_LastInput.Normalize();

            if (!m_IsJumping)
            {
                m_IsSprinting = Sprint.Value > 0.5f;

                var desiredVelocity =
                    m_LastInput * (m_IsSprinting ? SprintSpeed : Speed);

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
                var fwd = inputFrame * Vector3.forward;

                var qA = transform.rotation;
                var qB = Quaternion.LookRotation(
                    (InputForward == ForwardModes.Player &&
                     Vector3.Dot(fwd, m_CurrentVelocityXZ) < 0)
                        ? -m_CurrentVelocityXZ
                        : m_CurrentVelocityXZ,
                    UpDirection);

                var damping = justLanded ? 0 : Damping;

                transform.rotation = Quaternion.Slerp(
                    qA, qB, Damper.Damp(1, damping, Time.deltaTime));
            }

            if (PostUpdate != null)
            {
                var vel = Quaternion.Inverse(transform.rotation)
                          * m_CurrentVelocityXZ;

                vel.y = m_CurrentVelocityY;

                PostUpdate(
                    vel,
                    m_IsSprinting
                        ? JumpSpeed / SprintJumpSpeed
                        : 1);
            }
        }

        Vector3 UpDirection =>
            UpMode == UpModes.World ? Vector3.up : transform.up;

        Quaternion GetInputFrame(bool inputDirectionChanged)
        {
            var frame = Quaternion.identity;

            switch (InputForward)
            {
                case ForwardModes.Camera:
                    frame = Camera.transform.rotation;
                    break;

                case ForwardModes.Player:
                    return transform.rotation;

                case ForwardModes.World:
                    break;
            }

            var playerUp = transform.up;
            var up = frame * Vector3.up;

            const float BlendTime = 2f;

            m_TimeInHemisphere += Time.deltaTime;

            bool inTopHemisphere =
                Vector3.Dot(up, playerUp) >= 0;

            if (inTopHemisphere != m_InTopHemisphere)
            {
                m_InTopHemisphere = inTopHemisphere;
                m_TimeInHemisphere =
                    Mathf.Max(0, BlendTime - m_TimeInHemisphere);
            }

            var axis = Vector3.Cross(up, playerUp);

            if (axis.sqrMagnitude < 0.001f && inTopHemisphere)
                return frame;

            var angle =
                UnityVectorExtensions.SignedAngle(
                    up, playerUp, axis);

            var frameA =
                Quaternion.AngleAxis(angle, axis) * frame;

            Quaternion frameB = frameA;

            if (!inTopHemisphere || m_TimeInHemisphere < BlendTime)
            {
                frameB = frame * m_Upsidedown;

                var axisB =
                    Vector3.Cross(frameB * Vector3.up, playerUp);

                if (axisB.sqrMagnitude > 0.001f)
                    frameB =
                        Quaternion.AngleAxis(
                            180f - angle,
                            axisB) * frameB;
            }

            if (inputDirectionChanged)
                m_TimeInHemisphere = BlendTime;

            if (m_TimeInHemisphere >= BlendTime)
                return inTopHemisphere ? frameA : frameB;

            if (inTopHemisphere)
                return Quaternion.Slerp(
                    frameB,
                    frameA,
                    m_TimeInHemisphere / BlendTime);

            return Quaternion.Slerp(
                frameA,
                frameB,
                m_TimeInHemisphere / BlendTime);
        }

        bool ProcessJump()
        {
            bool justLanded = false;

            var now = Time.time;
            bool grounded = IsGrounded();

            m_CurrentVelocityY -= Gravity * Time.deltaTime;

            if (!m_IsJumping)
            {
                if (grounded && Jump.Value > 0.01f)
                {
                    m_IsJumping = true;
                    m_CurrentVelocityY =
                        m_IsSprinting
                            ? SprintJumpSpeed
                            : JumpSpeed;
                }

                if (!grounded &&
                    now - m_TimeLastGrounded >
                    kDelayBeforeInferringJump)
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

        void ApplyMotion()
        {
            if (m_Controller != null)
                m_Controller.Move(
                    (m_CurrentVelocityY * UpDirection
                     + m_CurrentVelocityXZ)
                    * Time.deltaTime);
            else
            {
                var pos =
                    transform.position
                    + m_CurrentVelocityXZ * Time.deltaTime;

                var up = UpDirection;

                var altitude =
                    GetDistanceFromGround(pos, up, 10);

                if (altitude < 0 && m_CurrentVelocityY <= 0)
                {
                    pos -= altitude * up;
                    m_CurrentVelocityY = 0;
                }
                else if (m_CurrentVelocityY < 0)
                {
                    var dy =
                        -m_CurrentVelocityY * Time.deltaTime;

                    if (dy > altitude)
                    {
                        pos -= altitude * up;
                        m_CurrentVelocityY = 0;
                    }
                }

                transform.position =
                    pos + m_CurrentVelocityY
                          * up * Time.deltaTime;
            }
        }

        float GetDistanceFromGround(
            Vector3 pos,
            Vector3 up,
            float max)
        {
            float kExtraHeight =
                m_Controller == null ? 2 : 0;

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

        public void Teleport(Vector3 newPos, Quaternion newRot)
        {
            if (m_Controller != null)
                m_Controller.enabled = false;

            var rot = transform.rotation;

            var rotDelta =
                newRot * Quaternion.Inverse(rot);

            m_CurrentVelocityXZ =
                rotDelta * m_CurrentVelocityXZ;

            transform.SetPositionAndRotation(
                newPos, newRot);

            if (m_Controller != null)
                m_Controller.enabled = true;
        }
    }
}
