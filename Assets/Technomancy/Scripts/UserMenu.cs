using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace intheclouds
{
    public class UserMenu : MonoBehaviour
    {
        public static UserMenu instance;
        public Button[] Tabs;
        public GameObject[] Pages;
        public GameObject[] controllerHints;
        public bool followPlayer;
        public bool menuIsOpen;
        public Toggle SmoothTurnToggle;
        public Toggle FollowToggle;
        public Toggle DebugModeToggle;
        private GameObject _spawnPoint;
        private GameObject _followThis;
        private LocalUserObjects _currentUserObjects;
        private GameObject _canvasGO;

        private void Start()
        {
            instance = this;
            _canvasGO = transform.GetChild(0).gameObject;
            _canvasGO.SetActive(false);
            
            UserSetup(LocalUserObjects.instance.PlayerStats);
            
            SmoothTurnToggle.SetIsOnWithoutNotify(LocalUserObjects.instance.ITCPlayerController.RotationType == RotationType.Smooth);
            FollowToggle.SetIsOnWithoutNotify(followPlayer);
            DebugModeToggle.SetIsOnWithoutNotify(Startup.instance.debugMode);

            transform.position = _spawnPoint.transform.position;
        }

        private void Update()
        {
            if (followPlayer)
            {
                transform.position = Vector3.Lerp(transform.position, _spawnPoint.transform.position, 5 * Time.deltaTime);
            }

            if (menuIsOpen)
            {
                transform.LookAt(2 * transform.position - _followThis.transform.position);
            }
        }

        public void UserSetup(PlayerStats player)
        {
            _currentUserObjects = player.LocalUserObjects;
            _spawnPoint = _currentUserObjects.userMenuSpawnPoint;
            _followThis = _currentUserObjects.Camera.gameObject;
        }

        public void ToggleMenu(bool forceShow = false)
        {
            if (!menuIsOpen || forceShow)
            {
                ShowMenu();
            }
            else
            {
                HideMenu();
            }

            menuIsOpen = !menuIsOpen;
        }

        private void ShowMenu()
        {
            transform.position = _spawnPoint.transform.position;
            _currentUserObjects.HVRPlayerInputs.UpdateInputs = false;
            HVRManager.Instance.ToggleHandGrabbers(false);
            _canvasGO.SetActive(true);
        }

        private void HideMenu()
        {
            _currentUserObjects.HVRPlayerInputs.UpdateInputs = true;
            HVRManager.Instance.ToggleHandGrabbers(true);
            _canvasGO.SetActive(false);
        }

        public void Toggle_SmoothTurn(bool smooth)
        {
            _currentUserObjects.ITCPlayerController.RotationType = smooth ? RotationType.Smooth : RotationType.Snap;
            Startup.SaveUserTurnSetting(smooth ? 0 : 1);
        }

        public void Toggle_DebugMode(bool toggle)
        {
            if (DebugModeToggle.isOn != toggle) // force update toggle UI in case toggled using keyboard
            {
                DebugModeToggle.SetIsOnWithoutNotify(toggle);
            }
            Startup.instance.debugMode = toggle;
            Startup.SaveDebugSetting(toggle ? 1 : 0);
        }

        // Currently only used when selecting tabs in menu. Can be called if want to show player specific page
        public void ChangeTab(int tabIndex)
        {
            for (var i = 0; i < Tabs.Length; i++)
            {
                Tabs[i].interactable = i != tabIndex;
                Pages[i].SetActive(i == tabIndex);
            }
        }

        public void Button_CalibrateHeight()
        {
            _currentUserObjects.HVRCameraRig.Calibrate();
        }

        public void Button_Standing()
        {
            var sitStandSetting = _currentUserObjects.HVRCameraRig.SitStanding;
            if (sitStandSetting == HVRSitStand.Sitting)
            {
                _currentUserObjects.HVRCameraRig.SetSitStandMode(HVRSitStand.PlayerHeight);
            }
        }

        public void Button_Seated()
        {
            var sitStandSetting = _currentUserObjects.HVRCameraRig.SitStanding;
            if (sitStandSetting == HVRSitStand.PlayerHeight)
            {
                _currentUserObjects.HVRCameraRig.SetSitStandMode(HVRSitStand.Sitting);
            }
        }

        public void Button_NextTurn()
        {
            if (GameManager.instance.state == GameState.CombatStart)
            {
                GameManager.instance.ForceNextTurn();
            }
        }

        public void Button_ControllerHints()
        {
            foreach (var controllerHint in controllerHints)
            {
                controllerHint.SetActive(!controllerHint.activeSelf);
            }
        }

        public void Button_ConfirmReturnToHub()
        {
            if (GameManager.instance.state == GameState.CombatStart)
            {
                GameManager.instance.EndCombat();
            }
            else
            {
                _currentUserObjects.PlayerStats.ResetPlayerStatus();
            }

            ToggleMenu();
            _currentUserObjects.ITCPlayerInputs.ResetUserMenuInputs();
            SceneLoader.instance.GoToSceneAsync("Celestial Hub");
        }

        public void Toggle_FollowPlayer(bool toggle)
        {
            followPlayer = toggle;
        }

        public void Button_ResetScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Button_ResetStats()
        {
            var playerStatsArray = FindObjectsOfType<PlayerStats>();
            foreach (var playerStats in playerStatsArray)
            {
                playerStats.CurrentHealth = playerStats.statsSO.maxHealth;
                playerStats.CurrentPoise = playerStats.statsSO.maxPoise;
                playerStats.CurrentMagicArmor = playerStats.statsSO.maxMagicArmor;
                playerStats.CurrentAP = playerStats.statsSO.startingAP;
                // if (playerStats.InCombat)
                // {
                //     playerStats.Turn = true;
                // }
            }

            Debug.Log("RESET STATS");
        }

        public void Button_ExplorationMode()
        {
            Debug.Log("Starting Exploration Mode...");
            Button_ResetStats();
            _currentUserObjects.PlayerMovementAP.enabled = false;
            _currentUserObjects.ITCPlayerController.MovementEnabled = true;
        }

        public void Button_StartPlayerTurn()
        {
            Debug.Log("Starting Player Turn... (not fully implemented yet)");
            Button_ResetStats();
            FindObjectOfType<PlayerStats>().Turn = true;
            _currentUserObjects.PlayerMovementAP.enabled = true;
            _currentUserObjects.PlayerMovementAP.StartTurn();
        }

        public void Button_StartEnemyTurn()
        {
            Debug.Log("Starting Enemy Turn... (not implemented yet)");
        }

        public void Button_ResetCenterBeacon()
        {
            _currentUserObjects.PlayerMovementAP.ResetCurrentPosition();
        }
    }
}