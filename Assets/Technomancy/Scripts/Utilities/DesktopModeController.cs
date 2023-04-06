using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SpatialTracking;

namespace intheclouds
{
    public class DesktopModeController : MonoBehaviour
    {
        public static DesktopModeController Instance;
        private Transform head;
        private Transform leftController;
        private Transform rightController;

        private void Awake()
        {
            Instance = this;

            if (!Startup.Instance.isDesktopMode)
            {
                enabled = false;
                return;
            }

            EnableDesktopControls();
        }

        private void Start()
        {
        }

        private void Update()
        {
            // todo: reference FallbackCameraController.cs in other project
            if (Mouse.current.rightButton.isPressed)
            {
            }
        }

        public void EnableDesktopControls()
        {
            head = LocalUserObjects.Instance.Camera.transform;
            leftController = LocalUserObjects.Instance.leftController;
            rightController = LocalUserObjects.Instance.rightController;

            head.GetComponent<TrackedPoseDriver>().enabled = false;
            leftController.GetComponent<TrackedPoseDriver>().enabled = false;
            rightController.GetComponent<TrackedPoseDriver>().enabled = false;
        }

        public void DisableDesktopControls()
        {
            leftController.GetComponent<TrackedPoseDriver>().enabled = true;
            rightController.GetComponent<TrackedPoseDriver>().enabled = true;
            head.GetComponent<TrackedPoseDriver>().enabled = true;
        }
    }
}