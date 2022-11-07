using HurricaneVR.Framework.Core.Utils;
using UnityEngine;

namespace intheclouds
{
    public class ImpactSFX : MonoBehaviour
    {
        public float impactSpeedRequired = 1f;
        public AudioClip impactClip;
        public float volume = 0.15f;
        public float maxPitch = 1;
        public float minPitch = 1;
        private AudioSource audioSource;
        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Left Hand") || collision.gameObject.CompareTag("Right Hand")) return;
            if (collision.relativeVelocity.magnitude > impactSpeedRequired)
            {
                if (!audioSource)
                {
                    audioSource = SFXPlayer.Instance.PlaySFXRandomPitch(impactClip, transform.position, minPitch, maxPitch, volume * (collision.relativeVelocity.magnitude * 0.5f), 20);
                }
                else
                {
                    if (!audioSource.isPlaying)
                    {
                        audioSource = SFXPlayer.Instance.PlaySFXRandomPitch(impactClip, transform.position, minPitch, maxPitch, volume * (collision.relativeVelocity.magnitude * 0.5f), 20);
                    }
                }
            }
        }
    }
}