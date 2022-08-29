using UnityEngine;

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
            if (rb.velocity.magnitude > highVelSwingTriggerSpeed)
            {
                if (swipeSFXAudioSource.isPlaying) return;
                swipeSFXAudioSource.pitch = Random.Range(1.2f, 1.3f);
                swipeSFXAudioSource.Play();
                // Debug.Log("fast swipe!");
            }
            else if (rb.velocity.magnitude > mediumVelSwingTriggerSpeed)
            {
                if (swipeSFXAudioSource.isPlaying) return;
                swipeSFXAudioSource.pitch = Random.Range(1.1f, 1.15f);
                swipeSFXAudioSource.Play();
                // Debug.Log("med swipe!");
            }
            else if (rb.velocity.magnitude > lowVelSwingTriggerSpeed)
            {
                if (swipeSFXAudioSource.isPlaying) return;
                swipeSFXAudioSource.pitch = Random.Range(1f, 1.05f);
                swipeSFXAudioSource.Play();
            }
        }
    }
}