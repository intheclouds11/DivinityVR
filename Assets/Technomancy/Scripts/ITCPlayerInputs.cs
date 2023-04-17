using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Core.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace intheclouds
{
    public class ITCPlayerInputs : MonoBehaviour
    {
        public AudioClip MenuClip;
        public GameObject MenuIcon;
        public GameObject DebugUI;
        public TextMeshProUGUI DebugStatusText;
        public float holdTimeRequired = 1;
        private float holdTimeLeftPrimaryButton;
        private float holdTimeLeftSecondaryButton;
        private GameManager gameManager;
        private bool triggeredPrimaryInput;
        private bool triggeredSecondaryInput;

        private void Awake()
        {
            gameManager = GameManager.Instance;
            DebugUI.SetActive(Debug.isDebugBuild);
        }

        private void Update()
        {
            CheckMenuButton();
            CheckEndTurnButton();
            if (DebugUI.activeInHierarchy)
            {
                CheckDeveloperDebugInputs();
                DebugStatusText.text = Startup.Instance.debugMode ? "Debug Mode On" : "Debug Mode Off";
            }
        }

        private void CheckDeveloperDebugInputs()
        {
            if (Keyboard.current.nKey.wasPressedThisFrame)
            {
                gameManager.ForceNextTurn();
                SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, gameManager.controlledPlayer.LocalUserObjects.Camera.transform.position, 1, 1, 10, false, false);
            }

            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                UserMenu.Instance.ToggleMenu();
            }
            
            if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.backquoteKey.wasPressedThisFrame)
            {
                UserMenu.Instance.Toggle_DebugMode(!Startup.Instance.debugMode);
            }
        }

        private void CheckMenuButton()
        {
            if (!UserMenu.Instance.menuIsOpen)
            {
                if (HVRInputManager.Instance.LeftController.SecondaryButtonState.Active && !triggeredSecondaryInput)
                {
                    if (holdTimeLeftSecondaryButton == 0)
                    {
                        MenuIcon.SetActive(true);
                        SFXPlayer.Instance.PlaySFX(MenuClip, gameManager.controlledPlayer.LocalUserObjects.Camera.transform.position, 1f, 0.5f, 10, false, false);
                    }
                    if (holdTimeLeftSecondaryButton > holdTimeRequired)
                    {
                        UserMenu.Instance.ToggleMenu();
                        MenuIcon.SetActive(false);
                        // SFXPlayer.Instance.PlaySFX(, gameManager.controlledPlayer.LocalUserObjects.Camera.transform.position, 0.7f, 0.5f, 10, false, false);
                        holdTimeLeftSecondaryButton = 0;
                        triggeredSecondaryInput = true;
                    }

                    holdTimeLeftSecondaryButton += Time.deltaTime;
                }
                else if (HVRInputManager.Instance.LeftController.SecondaryButtonState.JustDeactivated)
                {
                    MenuIcon.SetActive(false);
                    holdTimeLeftSecondaryButton = 0;
                    triggeredSecondaryInput = false;
                }
            }
            else if (HVRInputManager.Instance.LeftController.SecondaryButtonState.JustActivated)
            {
                UserMenu.Instance.ToggleMenu();
                MenuIcon.SetActive(false);
                SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, gameManager.controlledPlayer.LocalUserObjects.Camera.transform.position, 0.8f, 0.5f, 10, false, false);
            }
        }

        private void CheckEndTurnButton()
        {
            if (!gameManager.activeCombatant || (!gameManager.playerTurn && !Startup.Instance.debugMode)) return;
            
            if (HVRInputManager.Instance.LeftController.PrimaryButtonState.Active && !triggeredPrimaryInput)
            {
                if (holdTimeLeftPrimaryButton == 0)
                {
                    SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, gameManager.controlledPlayer.LocalUserObjects.Camera.transform.position, 0.8f, 0.5f, 10, false, false);
                }
                if (holdTimeLeftPrimaryButton > holdTimeRequired)
                {
                    SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, gameManager.controlledPlayer.LocalUserObjects.Camera.transform.position, 1, 0.5f, 10, false, false);
                    gameManager.ForceNextTurn();
                    holdTimeLeftPrimaryButton = 0;
                    triggeredPrimaryInput = true;
                }

                holdTimeLeftPrimaryButton += Time.deltaTime;
            }
            else if (HVRInputManager.Instance.LeftController.PrimaryButtonState.JustDeactivated)
            {
                holdTimeLeftPrimaryButton = 0;
                triggeredPrimaryInput = false;
            }
        }
    }
}