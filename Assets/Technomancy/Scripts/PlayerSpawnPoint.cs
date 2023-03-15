using UnityEngine;

namespace intheclouds
{
    public class PlayerSpawnPoint : MonoBehaviour
    {
        public bool startPosition;

        private void OnEnable()
        {
            SpawnManager.RegisterUserSpawnPoint(this);
        }

        private void OnDisable()
        {
            SpawnManager.UnregisterUserSpawnPoint(this);
        }
    }
}
