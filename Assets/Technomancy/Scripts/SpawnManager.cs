using System.Collections.Generic;
using Pathfinding.RVO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace intheclouds
{
    public class SpawnManager : MonoBehaviour
    {
        public static List<PlayerSpawnPoint> PlayerSpawnPoints { get; private set; } = new List<PlayerSpawnPoint>();

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            MovePlayerToStartingSpawnPoint();

            var playerRvoController = LocalUserObjects.instance.ITCPlayerController.GetComponent<RVOController>();

            if (FindFirstObjectByType<RVOSimulator>() && !playerRvoController.enabled)
            {
                playerRvoController.enabled = true;
            }
        }

        private void Start()
        {
            MovePlayerToStartingSpawnPoint();
        }

        public static void MovePlayerToStartingSpawnPoint()
        {
            var spawnPoint = GetStartingPlayerSpawnPoint().transform;
            LocalUserObjects.instance.ITCTeleporter.Teleport(spawnPoint.position, spawnPoint.forward);
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
            
            return null;
        }
    }
}
