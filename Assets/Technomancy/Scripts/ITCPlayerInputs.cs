using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace intheclouds
{
    public class ITCPlayerInputs : MonoBehaviour
    {
        public UserMenu menu;
        public float holdTimeRequired = 1;
        private float holdTimeLeftPrimaryButton;
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
            if (Startup.Instance.isDesktopMode && HVRInputManager.Instance.LeftController.PrimaryTouchButtonState.JustActivated)
            {
                gameManager.ForceNextTurn();
                SFXPlayer.Instance.PlaySFXAttach(SFXPlayer.Instance.clickSFX, gameManager.controlledPlayer.LocalUserObjects.Camera.transform, 1, 1);
            }
            else if (HVRInputManager.Instance.LeftController.PrimaryButtonState.Active)
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
                holdTimeLeftPrimaryButton = 0;
            }
        }
    }
}