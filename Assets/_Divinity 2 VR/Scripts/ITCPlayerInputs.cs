using System;
using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Core.Utils;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace intheclouds
{
    public class ITCPlayerInputs : MonoBehaviour
    {
        public UserMenu menu;
        public int holdTimeRequired = 1;
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
                SFXPlayer.Instance.PlaySFXAttach(SFXPlayer.Instance.clickSFX, gameManager.controlledPlayer.LocalUserObjects.Camera.transform, 1, 1);
            }
            else if (!primaryButtonTriggered && HVRInputManager.Instance.LeftController.PrimaryButtonState.Active)
            {
                if (holdTimeLeftPrimaryButton == 0)
                {
                    SFXPlayer.Instance.PlaySFXAttach(SFXPlayer.Instance.clickSFX, gameManager.controlledPlayer.LocalUserObjects.Camera.transform, 0.8f, 1);
                }
                if (holdTimeLeftPrimaryButton > holdTimeRequired)
                {
                    SFXPlayer.Instance.PlaySFXAttach(SFXPlayer.Instance.clickSFX, gameManager.controlledPlayer.LocalUserObjects.Camera.transform, 1, 1);
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