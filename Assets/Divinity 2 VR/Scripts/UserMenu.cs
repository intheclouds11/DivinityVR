using System.Collections.Generic;
using HurricaneVR.Framework.Core.Player;
using HurricaneVR.Framework.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace intheclouds
{
    public class UserMenu : MonoBehaviour
    {
        [Tooltip("For adjusting bindings, grip amount, etc.")]
        public LocalUserObjects currentUserObjects;
        public bool followPlayer;
        public GameObject player;
        public GameObject playerSelectButton;
        public LayoutGroup playerSelectButtonGroup;
        private List<GameObject> currentPlayerSelectButtons = new List<GameObject>();
        public GameObject followPoint;
        public bool menuIsOpen;
        private GameObject canvasGO;
        public GameObject[] controllerHints = new GameObject[2];

        private void Start()
        {
            menuIsOpen = transform.GetChild(0).gameObject.activeInHierarchy;
            canvasGO = transform.GetChild(0).gameObject;
            ListPlayers(); // need to call this anytime a player is added/removed from GameManager
        }

        private void Update()
        {
            if (followPlayer)
            {
                transform.position = Vector3.Lerp(transform.position, followPoint.transform.position, 5 * Time.deltaTime);
            }

            transform.LookAt(2 * transform.position - player.transform.position);
        }

        public void ToggleMenu()
        {
            if (!menuIsOpen)
            {
                transform.position = followPoint.transform.position;
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

        public void ListPlayers()
        {
            foreach (var playerStats in GameManager.Instance.players)
            {
                var playerButton = Instantiate(playerSelectButton, playerSelectButtonGroup.transform);
                playerButton.GetComponentInChildren<TextMeshProUGUI>().text = playerStats.Name;

                //todo: assign player to button so can switch to when clicked

                currentPlayerSelectButtons.Add(playerButton);
            }
        }

        public void Button_ChangeControlledCharacter()
        {
            
            
            var playerNameSwitchingTo = GetComponentInChildren<TextMeshProUGUI>().text;
            foreach (var p in GameManager.Instance.players)
            {
                if (p.Name == playerNameSwitchingTo)
                {
                    // if already that character, abort
                    if (p.Name == player.GetComponent<PlayerStats>().Name)
                    {
                        Debug.Log("Cannot swap. Player is already active");
                        break;
                    }

                    p.playerControlled = true;
                    Debug.Log($"Swapped controls to {p.Name}");
                }
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
                playerStats.currentHealth = playerStats.playerStatsSO.maxHealth;
                playerStats.currentPhysicalArmor = playerStats.playerStatsSO.maxPhysicalArmor;
                playerStats.currentMagicArmor = playerStats.playerStatsSO.maxMagicArmor;
                playerStats.currentAP = playerStats.playerStatsSO.maxAP;
                playerStats.turn = true;
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
            FindObjectOfType<PlayerStats>().turn = true;
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