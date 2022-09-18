using System;
using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.ControllerInput;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace intheclouds
{
    public class ITCPlayerInputs : MonoBehaviour
    {
        public UserMenu menu;
        public int holdTimeRequired = 2;
        private float holdTimeLeftPrimaryButton;
        private bool primaryButtonTriggered;
        private GameManager gameManager;

        private void Awake()
        {
            gameManager = GameManager.Instance;
        }

        private void Update()
        {
            CheckMenuButton();
            CheckEndTurnButton();
        }

        private void CheckMenuButton()
        {
            if (HVRInputManager.Instance.LeftController.SecondaryButtonState.JustActivated)
            {
                menu.ToggleMenu();
            }
        }

        private void CheckEndTurnButton()
        {
            if (Keyboard.current.nKey.wasPressedThisFrame)
            {
                gameManager.ForceNextTurn();
            }
            else if (!primaryButtonTriggered && HVRInputManager.Instance.LeftController.PrimaryButtonState.Active)
            {
                if (holdTimeLeftPrimaryButton > holdTimeRequired)
                {
                    gameManager.ForceNextTurn();
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