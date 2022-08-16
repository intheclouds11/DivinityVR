using System;
using System.Collections;
using System.Collections.Generic;
using intheclouds;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

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

        private void OnEnable()
        {
            transform.position = followPoint.transform.position;
        }

        public void Button_ResetScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Button_ResetStats()
        {
            var playerStatsArray = FindObjectsOfType<PlayerStatsSO>();
            foreach (var playerStats in playerStatsArray)
            {
                playerStats.currentHealth = playerStats.maxHealth;
                playerStats.currentAP = playerStats.maxAP;
            }
        }

        private void Update()
        {
            if (followPlayer)
            {
                transform.position = Vector3.Lerp(transform.position, followPoint.transform.position, 5 * Time.deltaTime);
            }

            transform.LookAt(2 * transform.position - player.transform.position);
        }

        public void Toggle_FollowPlayer()
        {
            if (!followPlayer)
            {
                // transform.SetParent(originalParent, true);
                followPlayer = true;
            }
            else
            {
                // transform.SetParent(transform.parent.parent, true);
                followPlayer = false;
            }
        }

        public void Button_StartPlayerTurn()
        {
            Debug.Log("Turn System not implemented yet");
        }

        public void Button_StartEnemyTurn()
        {
            Debug.Log("Turn System not implemented yet");
        }
    }
}