using UnityEngine;
using System.Collections.Generic;

namespace Unity.Cinemachine.Samples
{
    [ExecuteAlways]
    public class AimCameraRig : CinemachineCameraManagerBase
    {
        SimplePlayerAimController AimController;
        CinemachineVirtualCameraBase AimCamera;
        CinemachineVirtualCameraBase FreeCamera;

        // 👉 Aim hanya pakai klik kanan
        // 👉 Aim hanya pakai CTRL
bool IsAiming =>
    Input.GetMouseButton(1); // RMB = aim

    


        protected override void Start()
        {
            base.Start();

            for (int i = 0; i < ChildCameras.Count; ++i)
            {
                var cam = ChildCameras[i];
                if (!cam.isActiveAndEnabled)
                    continue;

                if (AimCamera == null
                    && cam.TryGetComponent<CinemachineThirdPersonAim>(out var aim)
                    && aim.NoiseCancellation)
                {
                    AimCamera = cam;
                    var player = AimCamera.Follow;
                    if (player != null)
                        AimController = player.GetComponentInChildren<SimplePlayerAimController>();
                }
                else if (FreeCamera == null)
                {
                    FreeCamera = cam;
                }
            }

            if (AimCamera == null)
                Debug.LogError("AimCameraRig: no valid CinemachineThirdPersonAim camera found among children");

            if (AimController == null)
                Debug.LogError("AimCameraRig: no valid SimplePlayerAimController target found");

            if (FreeCamera == null)
                Debug.LogError("AimCameraRig: no valid non-aiming camera found among children");
        }

        protected override CinemachineVirtualCameraBase ChooseCurrentCamera(Vector3 worldUp, float deltaTime)
        {
            var oldCam = (CinemachineVirtualCameraBase)LiveChild;
            var newCam = IsAiming ? AimCamera : FreeCamera;

            if (AimController != null && oldCam != newCam)
            {
                AimController.PlayerRotation = IsAiming
                    ? SimplePlayerAimController.CouplingMode.Coupled
                    : SimplePlayerAimController.CouplingMode.Decoupled;

                AimController.RecenterPlayer();
            }

            return newCam;
        }
    }
}
