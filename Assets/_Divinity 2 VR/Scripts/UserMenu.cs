using System.Collections.Generic;
using HurricaneVR.Framework.Core.Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace intheclouds
{
    public class UserMenu : MonoBehaviour
    {
        public static UserMenu Instance;
        public GameObject[] controllerHints = new GameObject[2];
        public bool followPlayer;
        public bool menuIsOpen;
        public GameObject playerSelectPrefab;
        public LayoutGroup playerSelectButtonGroup;
        public Toggle SmoothTurnToggle;
        public Toggle FollowToggle;
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
            SmoothTurnToggle.onValueChanged.AddListener(OnSmoothTurnChanged);
            FollowToggle.onValueChanged.AddListener(OnFollowPlayerChanged);
            ListPlayers(); // todo: call this anytime a player is added/removed from GameManager

            foreach (var player in GameManager.Instance.players)
            {
                localUserObjectsList.Add(player.transform.root.GetComponent<LocalUserObjects>());
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
            currentUserObjects = player.transform.root.GetComponent<LocalUserObjects>();
            spawnPoint = currentUserObjects.userMenuSpawnPoint;
            followThis = currentUserObjects.Camera.gameObject;
            CameraRigs.Add(currentUserObjects.HVRCameraRig);
        }

        public void ToggleMenu()
        {
            if (!menuIsOpen)
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

        // need to call this when player joins/leaves party
        public void ListPlayers()
        {
            foreach (var playerStats in GameManager.Instance.players)
            {
                var playerButton = Instantiate(playerSelectPrefab, playerSelectButtonGroup.transform);
                playerButton.GetComponentInChildren<TextMeshProUGUI>().text = playerStats.Name;
                currentPlayerSelectButtons.Add(playerButton);
            }
        }
        
        public void OnSmoothTurnChanged(bool smooth)
        {
            foreach (var userObjects in localUserObjectsList)
            {
                userObjects.HVRPlayerController.RotationType = smooth ? RotationType.Smooth : RotationType.Snap;
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

        public void Button_OnSitStandClicked()
        {
            foreach (var cameraRig in CameraRigs)
            {
                // Swap between Sitting and PlayerHeight (Standing scales camera eh...)
                var index = (int) cameraRig.SitStanding;
                if (index == 0)
                {
                    index = 2;
                }
                else
                {
                    index = 0;
                }

                cameraRig.SetSitStandMode((HVRSitStand) index);
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

        public void OnFollowPlayerChanged(bool toggle)
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
                playerStats.CurrentAP = playerStats.statsSO.maxAP;
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
            currentUserObjects.PlayerStats.ExplorationMode = true;
            currentUserObjects.HVRPlayerController.MovementEnabled = true;
        }

        public void Button_StartPlayerTurn()
        {
            Debug.Log("Starting Player Turn... (not fully implemented yet)");
            Button_ResetStats();
            FindObjectOfType<PlayerStats>().Turn = true;
            FindObjectOfType<PlayerStats>().ExplorationMode = false;
            currentUserObjects.PlayerMovementAP.enabled = true;
            currentUserObjects.PlayerMovementAP.StartTurn();
        }

        public void Button_StartEnemyTurn()
        {
            Debug.Log("Starting Enemy Turn... (not implemented yet)");
        }
    }
}