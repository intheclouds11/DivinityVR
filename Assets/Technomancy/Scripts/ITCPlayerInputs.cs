using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Core.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace intheclouds
{
    public class ITCPlayerInputs : MonoBehaviour
    {
        public GameObject DebugUI;
        public TextMeshProUGUI DebugStatusText;
        public float holdTimeRequired = 1;
        private float holdTimeLeftPrimaryButton;
        private GameManager gameManager;
        private bool triggered;

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
                SFXPlayer.Instance.PlaySFXAttach(SFXPlayer.Instance.clickSFX, gameManager.controlledPlayer.LocalUserObjects.Camera.transform, 1, 1);
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
            if (HVRInputManager.Instance.LeftController.SecondaryButtonState.JustActivated)
            {
                UserMenu.Instance.ToggleMenu();
            }
        }

        private void CheckEndTurnButton()
        {
            if (!gameManager.activeCombatant || (!gameManager.playerTurn && !Startup.Instance.debugMode)) return;
            
            if (HVRInputManager.Instance.LeftController.PrimaryButtonState.Active && !triggered)
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
                    triggered = true;
                }

                holdTimeLeftPrimaryButton += Time.deltaTime;
            }
            else if (HVRInputManager.Instance.LeftController.PrimaryButtonState.JustDeactivated)
            {
                holdTimeLeftPrimaryButton = 0;
                triggered = false;
            }
        }
    }
}