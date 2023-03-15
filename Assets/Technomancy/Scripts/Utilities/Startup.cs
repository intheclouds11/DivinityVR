using System;
using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.Core.UI;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.XR.Management;

namespace intheclouds
{
    public class Startup : MonoBehaviour
    {
        public static Startup Instance;
        public bool isDesktopMode;
        public InputSystemUIInputModule desktopModeInputModule;
        public HVRInputModule vrInputModule;


        private void Awake()
        {
            Instance = this;
            
            if (!isDesktopMode)
            {
                vrInputModule.enabled = true;
                StartCoroutine(LoadVRMode());
            }
            else
            {
                desktopModeInputModule.enabled = true;
            }
        }

        private IEnumerator LoadVRMode()
        {
            yield return StartCoroutine(XRGeneralSettings.Instance.Manager.InitializeLoader());
            XRGeneralSettings.Instance.Manager.StartSubsystems();
        }
    }
}
