using System.Collections.Generic;
using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Core.Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace intheclouds
{
    public class UserMenu : MonoBehaviour
    {
        public static UserMenu Instance;
        public Button[] Tabs;
        public GameObject[] Pages;
        public GameObject[] controllerHints;
        public bool followPlayer;
        public bool menuIsOpen;
        public GameObject playerSelectPrefab;
        public LayoutGroup playerSelectButtonGroup;
        public Toggle SmoothTurnToggle;
        public Toggle FollowToggle;
        public Toggle DebugModeToggle;
        private GameObject spawnPoint;
        private GameObject followThis;
        private LocalUserObjects currentUserObjects;
        // private List<LocalUserObjects> localUserObjectsList = new List<LocalUserObjects>();
        private List<GameObject> currentPlayerSelectButtons = new List<GameObject>();
        private GameObject canvasGO;

        private void Start()
        {
            Instance = this;
            canvasGO = transform.GetChild(0).gameObject;
            menuIsOpen = canvasGO.activeInHierarchy;
            
            UserSetup(LocalUserObjects.Instance.PlayerStats);
            
            SmoothTurnToggle.SetIsOnWithoutNotify(LocalUserObjects.Instance.HVRPlayerController.RotationType == RotationType.Smooth);
            FollowToggle.SetIsOnWithoutNotify(followPlayer);
            DebugModeToggle.SetIsOnWithoutNotify(Startup.Instance.debugMode);

            transform.position = spawnPoint.transform.position;
        }

        private void Update()
        {
            if (followPlayer)
            {
                transform.position = Vector3.Lerp(transform.position, spawnPoint.transform.position, 5 * Time.deltaTime);
            }

            if (menuIsOpen)
            {
                transform.LookAt(2 * transform.position - followThis.transform.position);
            }
        }

        public void UserSetup(PlayerStats player)
        {
            currentUserObjects = player.LocalUserObjects;
            spawnPoint = currentUserObjects.userMenuSpawnPoint;
            followThis = currentUserObjects.Camera.gameObject;
        }

        public void ToggleMenu(bool forceShow = false)
        {
            if (!menuIsOpen || forceShow)
            {
                transform.position = spawnPoint.transform.position;
                canvasGO.SetActive(true);
            }
            else
            {
                canvasGO.SetActive(false);
            }

            menuIsOpen = !menuIsOpen;
        }

        public void Toggle_SmoothTurn(bool smooth)
        {
            currentUserObjects.HVRPlayerController.RotationType = smooth ? RotationType.Smooth : RotationType.Snap;
            Startup.SaveUserTurnSetting(smooth ? 0 : 1);
        }

        public void Toggle_DebugMode(bool toggle)
        {
            Startup.Instance.debugMode = toggle;
            Startup.SaveDebugSetting(toggle ? 1 : 0);
        }

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
            currentUserObjects.HVRCameraRig.Calibrate();
        }

        public void Button_Standing()
        {
            var sitStandSetting = currentUserObjects.HVRCameraRig.SitStanding;
            if (sitStandSetting == HVRSitStand.Sitting)
            {
                currentUserObjects.HVRCameraRig.SetSitStandMode(HVRSitStand.PlayerHeight);
            }
        }

        public void Button_Seated()
        {
            var sitStandSetting = currentUserObjects.HVRCameraRig.SitStanding;
            if (sitStandSetting == HVRSitStand.PlayerHeight)
            {
                currentUserObjects.HVRCameraRig.SetSitStandMode(HVRSitStand.Sitting);
            }
        }

        public void Button_NextTurn()
        {
            if (GameManager.Instance.state == GameState.CombatStart)
            {
                GameManager.Instance.ForceNextTurn();
            }
        }

        public void Button_ControllerHints()
        {
            foreach (var controllerHint in controllerHints)
            {
                controllerHint.SetActive(!controllerHint.activeSelf);
            }
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
            currentUserObjects.PlayerMovementAP.enabled = false;
            currentUserObjects.HVRPlayerController.MovementEnabled = true;
        }

        public void Button_StartPlayerTurn()
        {
            Debug.Log("Starting Player Turn... (not fully implemented yet)");
            Button_ResetStats();
            FindObjectOfType<PlayerStats>().Turn = true;
            currentUserObjects.PlayerMovementAP.enabled = true;
            currentUserObjects.PlayerMovementAP.StartTurn();
        }

        public void Button_StartEnemyTurn()
        {
            Debug.Log("Starting Enemy Turn... (not implemented yet)");
        }
    }
}