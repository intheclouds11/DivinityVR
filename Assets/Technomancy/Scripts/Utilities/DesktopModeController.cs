using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Core;
using UnityEngine;

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
                return;
            }

            HVRManager.Instance.isDesktopMode = true;
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
