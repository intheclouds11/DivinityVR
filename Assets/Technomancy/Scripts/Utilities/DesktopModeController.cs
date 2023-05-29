using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SpatialTracking;

namespace intheclouds
{
    public class DesktopModeController : MonoBehaviour
    {
        public static DesktopModeController instance;
        private Transform _head;
        private Transform _leftController;
        private Transform _rightController;

        private void Awake()
        {
            instance = this;

            if (!Startup.instance.isDesktopMode)
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
            _head = LocalUserObjects.instance.Camera.transform;
            _leftController = LocalUserObjects.instance.leftController;
            _rightController = LocalUserObjects.instance.rightController;

            _head.GetComponent<TrackedPoseDriver>().enabled = false;
            _leftController.GetComponent<TrackedPoseDriver>().enabled = false;
            _rightController.GetComponent<TrackedPoseDriver>().enabled = false;
        }

        public void DisableDesktopControls()
        {
            _leftController.GetComponent<TrackedPoseDriver>().enabled = true;
            _rightController.GetComponent<TrackedPoseDriver>().enabled = true;
            _head.GetComponent<TrackedPoseDriver>().enabled = true;
        }
    }
}