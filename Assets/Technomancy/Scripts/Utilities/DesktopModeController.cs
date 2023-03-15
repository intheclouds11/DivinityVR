using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Shared;
using UnityEngine;
using UnityEngine.InputSystem.UI;

namespace intheclouds
{
    public class DesktopModeController : MonoBehaviour
    {
        private Transform head;
        private Transform leftController;
        private Transform rightController;
        
        private void Awake()
        {
            var inputSettings = HVRInputManager.Instance.KnucklesInputMap;
            if (!Startup.Instance.isDesktopMode)
            {
                inputSettings.GripUseAnalog = true;
                inputSettings.TriggerUseAnalog = true;
                enabled = false;
            }

            HVRInputManager.Instance.GetController(HVRHandSide.Left).isDesktopMode = true;
            HVRInputManager.Instance.GetController(HVRHandSide.Right).isDesktopMode = true;
            inputSettings.GripUseAnalog = false;
            inputSettings.TriggerUseAnalog = false;
        }

        private void Start()
        {
            head = LocalUserObjects.Instance.Camera.transform;
            leftController = LocalUserObjects.Instance.leftController;
            rightController = LocalUserObjects.Instance.rightController;
        }

        private void Update()
        {
        
        }
    }
}
