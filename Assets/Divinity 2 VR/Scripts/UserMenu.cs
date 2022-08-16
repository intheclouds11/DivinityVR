using System;
using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.Core.Player;
using intheclouds;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace intheclouds
{
    public class UserMenu : MonoBehaviour
    {
        // public Transform originalParent;
        // public Vector3 originalLocalPosition;
        // public Quaternion originalLocalRotation;
        public bool followPlayer;
        public GameObject player;
        public GameObject followPoint;
        public bool menuIsOpen;
        private GameObject canvasGO;
        public PlayerStats playerStats;
        public TextMeshProUGUI currentAPText;

        private void Start()
        {
            menuIsOpen = transform.GetChild(0).gameObject.activeInHierarchy;
            canvasGO = transform.GetChild(0).gameObject;
        }

        private void Update()
        {
            // Canvas placement
            if (followPlayer)
            {
                canvasGO.transform.position = Vector3.Lerp(canvasGO.transform.position, followPoint.transform.position, 5 * Time.deltaTime);
            }

            canvasGO.transform.LookAt(2 * canvasGO.transform.position - player.transform.position);
        }

        public void ToggleMenu()
        {
            if (!menuIsOpen)
            {
                canvasGO.transform.position = followPoint.transform.position;
                canvasGO.SetActive(true);
            }
            else
            {
                canvasGO.SetActive(false);
            }

            menuIsOpen = !menuIsOpen;
        }

        public void Toggle_FollowPlayer()
        {
            if (!followPlayer)
            {
                followPlayer = true;
            }
            else
            {
                followPlayer = false;
            }
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
                playerStats.currentHealth = playerStats.maxHealth;
                playerStats.currentPhysicalArmor = playerStats.maxPhysicalArmor;
                playerStats.currentMagicArmor = playerStats.maxMagicArmor;
                playerStats.currentAP = playerStats.maxAP;
            }
        }
        
        public void Button_ExplorationMode()
        {
            Debug.Log("Starting Exploration Mode...");
            transform.root.GetComponentInChildren<PlayerMovementAP>().enabled = false;
            FindObjectOfType<PlayerStats>().explorationMode = true;
            FindObjectOfType<HVRPlayerController>().MovementEnabled = true;
        }

        public void Button_StartPlayerTurn()
        {
            Debug.Log("Starting Player Turn... (not fully implemented yet)");
            Button_ResetStats();
            FindObjectOfType<PlayerStats>().playerTurnCombat = true;
            FindObjectOfType<PlayerStats>().explorationMode = false;
            transform.root.GetComponentInChildren<PlayerMovementAP>().enabled = true;
            transform.root.GetComponentInChildren<PlayerMovementAP>().StartTurnSetup();
        }

        public void Button_StartEnemyTurn()
        {
            Debug.Log("Starting Enemy Turn... (not implemented yet)");
        }
    }
}