using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace intheclouds
{
    public class TresureChest : MonoBehaviour
    {
        private AudioSource audioSource;
        public bool opened;
        public int gold = 12;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Hand"))
            {
                if (!opened)
                {
                    opened = true;
                    audioSource.Play();
                    collision.gameObject.transform.root.GetComponentInChildren<PlayerStats>().gold += gold;
                }
            }
        }
    }
}