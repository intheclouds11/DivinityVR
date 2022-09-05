using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Player;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    [RequireComponent(typeof(Transform))]
    public class WeaponSwipeSFX : MonoBehaviour
    {
        public AudioClip SwipeAudioClip;
        public float lowVelSwingTriggerSpeed;
        public float mediumVelSwingTriggerSpeed;
        public float highVelSwingTriggerSpeed;
        private Rigidbody rb;
        private bool isPlayingSFX;
        public CharacterController wielderCharacterController;
        private float previousDistance;
        private HVRGrabbable hvrGrabbable;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            hvrGrabbable = GetComponent<HVRGrabbable>();
        }

        private void Update()
        {
            if (isPlayingSFX || hvrGrabbable.IsSocketed) return;
            VelocityActions();
        }

        private void VelocityActions()
        {
            if (hvrGrabbable.IsHandGrabbed)
            {
                var wielderVelocityMag = wielderCharacterController.velocity.magnitude;
                var swordVelocityMag = rb.velocity.magnitude;
                var deltaVelocityMag = Mathf.Abs(wielderVelocityMag - swordVelocityMag);
                
                if (deltaVelocityMag > highVelSwingTriggerSpeed)
                {
                    isPlayingSFX = true;
                    SFXPlayer.Instance.PlaySFXRandomPitchAttach(SwipeAudioClip, transform, 1.1f, 1.2f, 0.5f, 20);
                    Invoke(nameof(DelayNextSFX), 0.4f);
                    Debug.Log("fast swipe!");
                }
                else if (deltaVelocityMag > mediumVelSwingTriggerSpeed)
                {
                    isPlayingSFX = true;
                    SFXPlayer.Instance.PlaySFXRandomPitchAttach(SwipeAudioClip, transform, 1.0f, 1.1f, 0.4f, 20);
                    Invoke(nameof(DelayNextSFX), 0.4f);
                    Debug.Log("med swipe!");
                }
                else if (deltaVelocityMag > lowVelSwingTriggerSpeed)
                {
                    isPlayingSFX = true;
                    SFXPlayer.Instance.PlaySFXRandomPitchAttach(SwipeAudioClip, transform, 0.9f, 1.0f, 0.2f, 20);
                    Invoke(nameof(DelayNextSFX), 0.4f);
                    Debug.Log("slow swipe!");
                }
            }
            else
            {
                if (rb.velocity.magnitude > highVelSwingTriggerSpeed)
                {
                    isPlayingSFX = true;
                    SFXPlayer.Instance.PlaySFXRandomPitchAttach(SwipeAudioClip, transform, 1.1f, 1.2f, 0.5f, 20);
                    Invoke(nameof(DelayNextSFX), 0.4f);
                    Debug.Log("fast swipe!");
                }
                else if (rb.velocity.magnitude > mediumVelSwingTriggerSpeed)
                {
                    isPlayingSFX = true;
                    SFXPlayer.Instance.PlaySFXRandomPitchAttach(SwipeAudioClip, transform, 1.0f, 1.1f, 0.4f, 20);
                    Invoke(nameof(DelayNextSFX), 0.4f);
                    Debug.Log("med swipe!");
                }
                else if (rb.velocity.magnitude > lowVelSwingTriggerSpeed)
                {
                    isPlayingSFX = true;
                    SFXPlayer.Instance.PlaySFXRandomPitchAttach(SwipeAudioClip, transform, 0.9f, 1.0f, 0.2f, 20);
                    Invoke(nameof(DelayNextSFX), 0.4f);
                    Debug.Log("slow swipe!");
                }
            }

        }

        private void DelayNextSFX()
        {
            isPlayingSFX = false;
        }
    }
}