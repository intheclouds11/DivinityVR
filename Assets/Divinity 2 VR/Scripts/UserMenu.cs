using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace intheclouds
{
    public class UserMenu : MonoBehaviour
    {
        public static UserMenu Instance;
        public GameObject[] controllerHints = new GameObject[2];
        public GameObject playerSelectButton;
        public bool followPlayer;
        public bool menuIsOpen;
        public GameObject spawnPoint;
        public GameObject followThis;
        public LocalUserObjects currentUserObjects;
        public LayoutGroup playerSelectButtonGroup;
        private List<GameObject> currentPlayerSelectButtons = new List<GameObject>();
        private GameObject canvasGO;

        private void Start()
        {
            Instance = this;
            menuIsOpen = transform.GetChild(0).gameObject.activeInHierarchy;
            canvasGO = transform.GetChild(0).gameObject;
            ListPlayers(); // todo: call this anytime a player is added/removed from GameManager

            foreach (var player in GameManager.Instance.players)
            {
                if (player.playerControlled)
                {
                    UserSetup(player);
                }
            }
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

        public void Button_NextTurn()
        {
            if (GameManager.Instance.state == GameState.CombatStart)
            {
                GameManager.Instance.nextTurn = true;
            }
        }

        // need to call this when player joins/leaves party
        public void ListPlayers()
        {
            foreach (var playerStats in GameManager.Instance.players)
            {
                var playerButton = Instantiate(playerSelectButton, playerSelectButtonGroup.transform);
                playerButton.GetComponentInChildren<TextMeshProUGUI>().text = playerStats.Name;
                currentPlayerSelectButtons.Add(playerButton);
            }
        }

        public void Button_ControllerHints()
        {
            foreach (var controllerHint in controllerHints)
            {
                controllerHint.SetActive(!controllerHint.activeSelf);
            }
        }

        public void Toggle_FollowPlayer()
        {
            followPlayer = !followPlayer;
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
                playerStats.Turn = true;
            }

            Debug.Log("RESET STATS");
        }

        public void Button_ExplorationMode()
        {
            Debug.Log("Starting Exploration Mode...");
            Button_ResetStats();
            currentUserObjects.PlayerMovementAP.enabled = false;
            currentUserObjects.PlayerStats.explorationMode = true;
            currentUserObjects.HVRPlayerController.MovementEnabled = true;
        }

        public void Button_StartPlayerTurn()
        {
            Debug.Log("Starting Player Turn... (not fully implemented yet)");
            Button_ResetStats();
            FindObjectOfType<PlayerStats>().Turn = true;
            FindObjectOfType<PlayerStats>().explorationMode = false;
            currentUserObjects.PlayerMovementAP.enabled = true;
            currentUserObjects.PlayerMovementAP.StartTurn();
        }

        public void Button_StartEnemyTurn()
        {
            Debug.Log("Starting Enemy Turn... (not implemented yet)");
        }
    }
}