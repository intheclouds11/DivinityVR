using System;
using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.Core.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace intheclouds
{
    public class ImpactSFX : MonoBehaviour
    {
        public float impactSpeedRequired = 1f;
        public AudioClip impactClip;
        public float volume = 0.15f;
        private AudioSource audioSource;
        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Left Hand") || collision.gameObject.CompareTag("Right Hand")) return;
            if (collision.relativeVelocity.magnitude > impactSpeedRequired)
            {
                if (!audioSource)
                {
                    audioSource = SFXPlayer.Instance.PlaySFXRandomPitch(impactClip, transform.position, 1f, 1f, volume * (collision.relativeVelocity.magnitude * 0.5f), 20);
                }
                else
                {
                    if (!audioSource.isPlaying)
                    {
                        audioSource = SFXPlayer.Instance.PlaySFXRandomPitch(impactClip, transform.position, 1f, 1f, volume * (collision.relativeVelocity.magnitude * 0.5f), 20);
                    }
                }
            }
        }
    }
}