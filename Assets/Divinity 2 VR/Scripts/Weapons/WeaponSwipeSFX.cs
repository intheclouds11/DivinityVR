using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    [RequireComponent(typeof(Transform))]
    public class WeaponSwipeSFX : MonoBehaviour
    {
        public float lowVelSwingTriggerSpeed;
        public float mediumVelSwingTriggerSpeed;
        public float highVelSwingTriggerSpeed;
        public AudioSource swipeSFXAudioSource;
        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            VelocityActions();
        }

        private void VelocityActions()
        {
            if (rb.velocity.magnitude > lowVelSwingTriggerSpeed)
            {
                if (!swipeSFXAudioSource.isPlaying)
                {
                    swipeSFXAudioSource.Play();
                    Debug.Log("small swipe!");
                }
            }
            else if (rb.velocity.magnitude > mediumVelSwingTriggerSpeed)
            {
                if (!swipeSFXAudioSource.isPlaying)
                {
                    swipeSFXAudioSource.Play();
                    Debug.Log("med swipe!");
                }
            }
            else if (rb.velocity.magnitude > highVelSwingTriggerSpeed)
            {
                if (!swipeSFXAudioSource.isPlaying)
                {
                    swipeSFXAudioSource.Play();
                    Debug.Log("fast swipe!");
                }
            }
        }
    }
}