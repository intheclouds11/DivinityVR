using System.Collections.Generic;
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
        public GameObject[] controllerHints = new GameObject[2];
        public bool followPlayer;
        public bool menuIsOpen;
        public GameObject playerSelectPrefab;
        public LayoutGroup playerSelectButtonGroup;
        public Toggle SmoothTurnToggle;
        public Toggle FollowToggle;
        public Toggle DebugEndAnyTurn;
        private List<HVRCameraRig> CameraRigs = new List<HVRCameraRig>();
        private GameObject spawnPoint;
        private GameObject followThis;
        private LocalUserObjects currentUserObjects;
        private List<LocalUserObjects> localUserObjectsList = new List<LocalUserObjects>();
        private List<GameObject> currentPlayerSelectButtons = new List<GameObject>();
        private GameObject canvasGO;

        private void Start()
        {
            Instance = this;
            canvasGO = transform.GetChild(0).gameObject;
            menuIsOpen = canvasGO.activeInHierarchy;
            DebugEndAnyTurn.isOn = Startup.Instance.debug_endAnyTurn;
            SmoothTurnToggle.isOn = LocalUserObjects.Instance.HVRPlayerController.RotationType == RotationType.Smooth;
            FollowToggle.isOn = followPlayer;

            foreach (var player in GameManager.Instance.players)
            {
                localUserObjectsList.Add(player.transform.GetComponent<LocalUserObjects>());
                if (player.PlayerControlled)
                {
                    UserSetup(player);
                }
            }

            transform.position = spawnPoint.transform.position;
        }

        private void Update()
        {
            if (followPlayer)
            {
                transform.position = Vector3.Lerp(transform.position, spawnPoint.transform.position, 5 * Time.deltaTime);
            }

            transform.LookAt(2 * transform.position - followThis.transform.position);
        }

        public void UserSetup(PlayerStats player)
        {
            currentUserObjects = player.LocalUserObjects;
            spawnPoint = currentUserObjects.userMenuSpawnPoint;
            followThis = currentUserObjects.Camera.gameObject;
            CameraRigs.Add(currentUserObjects.HVRCameraRig);
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
            foreach (var userObjects in localUserObjectsList)
            {
                userObjects.HVRPlayerController.RotationType = smooth ? RotationType.Smooth : RotationType.Snap;
            }
        }

        public void Toggle_EndAnyTurn(bool toggle)
        {
            Startup.Instance.debug_endAnyTurn = toggle;
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
            foreach (var cameraRig in CameraRigs)
            {
                if (cameraRig)
                {
                    cameraRig.Calibrate();
                }
            }
        }

        public void Button_Standing()
        {
            foreach (var cameraRig in CameraRigs)
            {
                var sitStandSetting = cameraRig.SitStanding;
                if (sitStandSetting == HVRSitStand.Sitting)
                {
                    cameraRig.SetSitStandMode(HVRSitStand.PlayerHeight);
                }
            }
        }
        
        public void Button_Seated()
        {
            foreach (var cameraRig in CameraRigs)
            {
                var sitStandSetting = cameraRig.SitStanding;
                if (sitStandSetting == HVRSitStand.PlayerHeight)
                {
                    cameraRig.SetSitStandMode(HVRSitStand.Sitting);
                }
            }
        }

        public void Button_NextTurn()
        {
            if (GameManager.Instance.state == GameState.CombatStart)
            {
                GameManager.Instance.NextTurn = true;
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
                if (playerStats.InCombat)
                {
                    playerStats.Turn = true;
                }
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