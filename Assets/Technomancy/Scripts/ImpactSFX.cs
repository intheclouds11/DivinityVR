using HurricaneVR.Framework.Components;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class ImpactSFX : MonoBehaviour
    {
        [FormerlySerializedAs("impactSpeedRequired")]
        public float impactForceRequired = 0.1f;
        public AudioClip impactClip;
        public float maxVolume = 0.5f;
        public float maxPitch = 1;
        public float minPitch = 1;
        private AudioSource audioSource;
        private HVRCollisionEvents collisionEvents;
        

        private void Start()
        {
            collisionEvents = GetComponent<HVRCollisionEvents>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            var relativeVelocity = collision.relativeVelocity.magnitude;
            if (collisionEvents && relativeVelocity >= collisionEvents.VelocityThreshold)
            {
                return; // Prevent impact sfx playing same time as destroy sfx
            }

            // var force = Vector3.Dot(collision.contacts[0].normal, collision.relativeVelocity) * collision.rigidbody.mass;
            if (relativeVelocity > impactForceRequired)
            {
                var pitch = Mathf.Clamp(relativeVelocity, minPitch, maxPitch);
                var vol = Mathf.Clamp(relativeVelocity * 0.1f, 0, maxVolume);
                if (!audioSource)
                {
                    audioSource = SFXPlayer.Instance.PlaySFX(impactClip, transform.position, pitch, vol, 20);
                }
                else
                {
                    if (!audioSource.isPlaying)
                    {
                        audioSource = SFXPlayer.Instance.PlaySFX(impactClip, transform.position, pitch, vol, 20);
                    }
                }
            }
        }
    }
}