using System;
using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.ControllerInput;
using UnityEditor.UI;
using UnityEngine;

namespace intheclouds
{
    public class ITCPlayerInputs : MonoBehaviour
    {
        public UserMenu menu;
        public bool debugInteractions;
        public static ITCPlayerInputs Instance;

        private void Start()
        {
            Instance = this;
        }

        private void Update()
        {
            if (HVRInputManager.Instance.LeftController.SecondaryButtonState.JustActivated)
            {
                menu.ToggleMenu();
            }
        }
    }
}