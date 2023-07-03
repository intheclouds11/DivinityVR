using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Core.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;

namespace intheclouds
{
    public class ITCPlayerInputs : MonoBehaviour
    {
        public AudioClip MenuClip;
        public GameObject MenuIcon;
        public GameObject DebugUI;
        public TextMeshProUGUI DebugStatusText;
        public float holdTimeRequired = 1;
        private float _holdTimeLeftPrimaryButton;
        private float _holdTimeLeftSecondaryButton;
        private GameManager _gameManager;
        private bool _triggeredPrimaryInput;
        private bool _triggeredSecondaryInput;

        public void ResetUserMenuInputs()
        {
            _triggeredSecondaryInput = false;
            _holdTimeLeftSecondaryButton = 0;
        }

        private void Awake()
        {
            _gameManager = GameManager.instance;
            DebugUI.SetActive(Debug.isDebugBuild);
            MenuIcon.SetActive(false);
        }

        private void Update()
        {
            CheckMenuButton();
            CheckEndTurnButton();
            if (DebugUI.activeInHierarchy)
            {
                CheckDeveloperDebugInputs();
                DebugStatusText.text = Startup.instance.debugMode ? "Debug Mode On" : "Debug Mode Off";
            }
        }

        private void CheckDeveloperDebugInputs()
        {
            if (Keyboard.current.nKey.wasPressedThisFrame)
            {
                _gameManager.ForceNextTurn();
                SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, _gameManager.controlledPlayer.LocalUserObjects.Camera.transform.position, 1, 1, 10, false, false);
            }

            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                UserMenu.instance.ToggleMenu();
            }

            if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.backquoteKey.wasPressedThisFrame)
            {
                UserMenu.instance.Toggle_DebugMode(!Startup.instance.debugMode);
            }
        }

        private void CheckMenuButton()
        {
            if (!UserMenu.instance.menuIsOpen)
            {
                if (LocalUserObjects.instance.HVRPlayerInputs.MenuButtonState.Active && !_triggeredSecondaryInput)
                {
                    if (_holdTimeLeftSecondaryButton == 0)
                    {
                        MenuIcon.SetActive(true);
                        SFXPlayer.Instance.PlaySFX(MenuClip, _gameManager.controlledPlayer.LocalUserObjects.Camera.transform.position, 1f, 0.5f, 10, false, false);
                    }

                    if (_holdTimeLeftSecondaryButton > holdTimeRequired)
                    {
                        UserMenu.instance.ToggleMenu();
                        MenuIcon.SetActive(false);
                        _holdTimeLeftSecondaryButton = 0;
                        _triggeredSecondaryInput = true;
                        return;
                    }

                    _holdTimeLeftSecondaryButton += Time.deltaTime;
                }
                else if (LocalUserObjects.instance.HVRPlayerInputs.MenuButtonState.JustDeactivated)
                {
                    MenuIcon.SetActive(false);
                    _holdTimeLeftSecondaryButton = 0;
                    _triggeredSecondaryInput = false;
                }
            }
            else if (LocalUserObjects.instance.HVRPlayerInputs.MenuButtonState.JustActivated)
            {
                UserMenu.instance.ToggleMenu();
                MenuIcon.SetActive(false);
                SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, _gameManager.controlledPlayer.LocalUserObjects.Camera.transform.position, 0.8f, 0.5f, 10, false,
                    false);
            }
        }

        private void CheckEndTurnButton()
        {
            if (LocalUserObjects.instance.HVRPlayerInputs.SkipButtonState.JustDeactivated || LocalUserObjects.instance.HVRPlayerInputs.MenuButtonState.Active)
            {
                _holdTimeLeftPrimaryButton = 0;
                _triggeredPrimaryInput = false;
                return;
            }

            if (!_gameManager.activeCombatant || (!LocalUserObjects.instance.PlayerStats.Turn && !Startup.instance.debugMode))
            {
                return;
            }

            if (LocalUserObjects.instance.HVRPlayerInputs.SkipButtonState.Active && !_triggeredPrimaryInput)
            {
                if (_holdTimeLeftPrimaryButton == 0)
                {
                    SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, _gameManager.controlledPlayer.LocalUserObjects.Camera.transform.position, 0.8f, 0.5f, 10, false,
                        false);
                }

                if (_holdTimeLeftPrimaryButton > holdTimeRequired)
                {
                    SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, _gameManager.controlledPlayer.LocalUserObjects.Camera.transform.position, 1, 0.5f, 10, false,
                        false);
                    _gameManager.ForceNextTurn();
                    _holdTimeLeftPrimaryButton = 0;
                    _triggeredPrimaryInput = true;
                }

                _holdTimeLeftPrimaryButton += Time.deltaTime;
            }
        }
    }
}