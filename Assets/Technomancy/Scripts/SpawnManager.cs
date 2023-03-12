using System;
using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.Core.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace intheclouds
{
    public class SpawnManager : MonoBehaviour
    {
        public static List<PlayerSpawnPoint> PlayerSpawnPoints { get; private set; } = new List<PlayerSpawnPoint>();

        private void Awake()
        {
            SceneManager.sceneLoaded += MovePlayerToStartingSpawnPoint;
        }

        private void MovePlayerToStartingSpawnPoint(Scene arg0, LoadSceneMode arg1)
        {
            MovePlayerToStartingSpawnPoint();
        }

        private void MovePlayerToStartingSpawnPoint()
        {
            var spawnPoint = GetStartingPlayerSpawnPoint().transform;
            LocalUserObjects.Instance.HVRPlayerController.GetComponent<HVRTeleporter>().Teleport(spawnPoint.position, spawnPoint.forward);
        }

        public static void RegisterUserSpawnPoint(PlayerSpawnPoint playerSpawnPoint)
        {
            PlayerSpawnPoints.Add(playerSpawnPoint);
        }
        
        public static void UnregisterUserSpawnPoint(PlayerSpawnPoint playerSpawnPoint)
        {
            PlayerSpawnPoints.Remove(playerSpawnPoint);
        }

        public static PlayerSpawnPoint GetStartingPlayerSpawnPoint()
        {
            foreach (var playerSpawnPoint in PlayerSpawnPoints)
            {
                if (playerSpawnPoint.startPosition)
                {
                    return playerSpawnPoint;
                }
            }

            Debug.LogError("No starting spawn point found!");

            return null;
        }
    }
}
