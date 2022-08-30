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
        public int holdTimeRequired = 2;
        private float holdTimeLeftPrimaryButton;
        private bool primaryButtonTriggered;
        private PlayerStats playerStats;
        public static ITCPlayerInputs Instance;

        private void Start()
        {
            Instance = this;
            playerStats = transform.root.GetComponent<LocalUserObjects>().PlayerStats;
        }

        private void Update()
        {
            if (HVRInputManager.Instance.LeftController.SecondaryButtonState.JustActivated)
            {
                menu.ToggleMenu();
            }

            if (!primaryButtonTriggered && HVRInputManager.Instance.LeftController.PrimaryButtonState.Active)
            {
                if (holdTimeLeftPrimaryButton > holdTimeRequired)
                {
                    playerStats.Turn = false;
                    primaryButtonTriggered = true;
                    holdTimeLeftPrimaryButton = 0;
                }

                holdTimeLeftPrimaryButton += Time.deltaTime;
            }
            else if (HVRInputManager.Instance.LeftController.PrimaryButtonState.JustDeactivated)
            {
                primaryButtonTriggered = false;
                holdTimeLeftPrimaryButton = 0;
            }
        }
    }
}