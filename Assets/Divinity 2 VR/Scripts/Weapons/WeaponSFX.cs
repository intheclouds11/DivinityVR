using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    [RequireComponent(typeof(Transform))]
    public class WeaponSFX : MonoBehaviour
    {
        public float lightHitTriggerSpeed;
        public float mediumHitTriggerSpeed;
        public float fastHitTriggerSpeed;
        public float lowVelSwingTriggerSpeed;
        public float mediumVelSwingTriggerSpeed;
        public float highVelSwingTriggerSpeed;
        public AudioSource hitSFXAudioSource;
        public AudioSource swingSFXAudioSource;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            VelocityActions();
        }

        private void OnCollisionEnter(Collision collision)
        {
            CollisionSpeedActions(collision);
        }

        private void VelocityActions()
        {
            if (rb.velocity.magnitude > lowVelSwingTriggerSpeed)
            {
                if (!swingSFXAudioSource.isPlaying)
                {
                    swingSFXAudioSource.Play();
                    Debug.Log("small swipe!");
                }
            }
            else if (rb.velocity.magnitude > mediumVelSwingTriggerSpeed)
            {
                if (!swingSFXAudioSource.isPlaying)
                {
                    swingSFXAudioSource.Play();
                    Debug.Log("med swipe!");
                }
            }
            else if (rb.velocity.magnitude > highVelSwingTriggerSpeed)
            {
                if (!swingSFXAudioSource.isPlaying)
                {
                    swingSFXAudioSource.Play();
                    Debug.Log("fast swipe!");
                }
            }
        }

        private void CollisionSpeedActions(Collision collision)
        {
            if (collision.relativeVelocity.magnitude > lightHitTriggerSpeed)
            {
                if (collision.gameObject.CompareTag("Enemy"))
                {
                    // Play light stab sound
                }
                else if (collision.gameObject.CompareTag("Sword"))
                {
                    hitSFXAudioSource.Play(); // TODO: PlayOneShotClip light cling sfx
                }
                else
                {
                    hitSFXAudioSource.Play(); // generic light hit
                }
            }

            if (collision.relativeVelocity.magnitude > mediumHitTriggerSpeed)
            {
                if (collision.gameObject.CompareTag("Enemy"))
                {
                    // Play medium stab sound
                }
                else if (collision.gameObject.CompareTag("Sword"))
                {
                    hitSFXAudioSource.Play(); // TODO: turn this into PlayOneShotClip medium sfx
                }
                else
                {
                    hitSFXAudioSource.Play(); // generic medium hit
                }
            }

            if (collision.relativeVelocity.magnitude > fastHitTriggerSpeed)
            {
                if (collision.gameObject.CompareTag("Enemy"))
                {
                    // Play fast stab sound
                }
                else if (collision.gameObject.CompareTag("Sword"))
                {
                    hitSFXAudioSource.Play(); // TODO: turn this into PlayOneShotClip fast sfx
                }
                else
                {
                    hitSFXAudioSource.Play(); // generic fast hit
                }
            }
        }
    }
}