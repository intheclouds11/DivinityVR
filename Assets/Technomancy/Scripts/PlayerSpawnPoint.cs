using System;
using System.Collections;
using System.Collections.Generic;
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

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
