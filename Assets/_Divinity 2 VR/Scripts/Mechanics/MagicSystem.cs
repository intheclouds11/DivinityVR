using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class MagicSystem : MonoBehaviour
    {
        private Vector3 spawnPosition;
        private Quaternion spawnRotation;
        private Transform spawnParent;
        public GameObject magicSlots;
        public Magic selectedMagic;
        public GameObject spawnedMagic;

        public GameObject description;
        public LocalUserObjects playerLUOs;
        private HVRHandGrabber leftHandGrabber;
        private HVRHandGrabber rightHandGrabber;
        public HVRHandGrabber Grabber { get; set; }
        public HVRGrabbable Grabbable;


        private void Awake()
        {
            playerLUOs = transform.root.GetComponent<LocalUserObjects>();
            leftHandGrabber = playerLUOs.leftHandPhysics.GetComponent<HVRHandGrabber>();
            rightHandGrabber = playerLUOs.rightHandPhysics.GetComponent<HVRHandGrabber>();
            spawnPosition = transform.localPosition;
            spawnRotation = transform.localRotation;
            spawnParent = transform.parent;

            if (magicSlots.activeInHierarchy)
            {
                magicSlots.SetActive(false);
            }
        }

        void Update()
        {
            // Selection
            CheckTouchPadPressed();

            // Activate
            if (selectedMagic)
            {
                CheckMagicActivation();
            }
        }

        private void CheckMagicActivation()
        {
            if (!spawnedMagic)
            {
                if (playerLUOs.HVRPlayerInputs.LeftController.TriggerButtonState.JustActivated
                    && playerLUOs.HVRPlayerInputs.LeftController.GripButtonState.Active && !leftHandGrabber.TriggerHoverTarget)
                {
                    SpawnMagic(true);
                }
                else if (playerLUOs.HVRPlayerInputs.RightController.TriggerButtonState.JustActivated
                         && playerLUOs.HVRPlayerInputs.RightController.GripButtonState.Active && !rightHandGrabber.TriggerHoverTarget)
                {
                    SpawnMagic(false);
                }
            }
        }

        private void SpawnMagic(bool leftHand)
        {
            if (leftHand)
            {
                spawnedMagic = Instantiate(selectedMagic.gameObject, playerLUOs.leftHandPalm.transform.position, Quaternion.identity);
                Grabber = playerLUOs.leftHandPhysics.GetComponent<HVRHandGrabber>();
            }
            else
            {
                spawnedMagic = Instantiate(selectedMagic.gameObject, playerLUOs.rightHandPalm.transform.position, Quaternion.identity);
                Grabber = playerLUOs.rightHandPhysics.GetComponent<HVRHandGrabber>();
            }

            spawnedMagic.SetActive(true);
            spawnedMagic.GetComponent<Fireball>().caster = playerLUOs.PlayerStats;
            Grabbable = spawnedMagic.GetComponent<HVRGrabbable>();
            Grabber.TryGrab(Grabbable);
        }

        private void CheckTouchPadPressed()
        {
            if (playerLUOs.HVRPlayerInputs.LeftController.TrackpadButtonState.JustActivated)
            {
                ShowSelector(playerLUOs.leftHandMagicSelectorSpawn.transform);
            }

            else if (playerLUOs.HVRPlayerInputs.LeftController.TrackpadButtonState.JustDeactivated)
            {
                HideSelector();
            }
        }

        public void ShowSelector(Transform spawnPoint)
        {
            transform.position = spawnPoint.position;
            var newEulerAngles = spawnPoint.eulerAngles;
            newEulerAngles = new Vector3(0, newEulerAngles.y, 0);
            transform.eulerAngles = newEulerAngles;

            magicSlots.SetActive(true);
            if (description.transform.childCount == 1)
            {
                description.SetActive(true);
            }

            playerLUOs.handAugmentHighlight.highlighted = true;
        }

        public void HideSelector()
        {
            if (!selectedMagic)
            {
                playerLUOs.handAugmentHighlight.overlayColor = playerLUOs.PlayerStats.statsSO.baseHandAugmentColor;
                playerLUOs.handAugmentHighlight.SetGlowColor(playerLUOs.PlayerStats.statsSO.baseHandAugmentColor);
                playerLUOs.handAugmentHighlight.highlighted = false;
            }

            magicSlots.SetActive(false);
            if (description.transform.childCount == 1)
            {
                description.SetActive(false);
            }
        }

        public void DequipMagic()
        {
            selectedMagic.gameObject.SetActive(false);
            selectedMagic = null;
            Destroy(description.transform.GetChild(0).gameObject);
        }

        // This is a good example for tapping button interaction

        // private void CheckTouchPadTouched()
        // {
        //     if (selectedMagic)
        //     {
        //         if (player.LocalUserObjects.HVRPlayerInputs.LeftController.TrackPadTouchState.JustActivated)
        //         {
        //             deselectCooldownTimer += 1;
        //
        //             if (deselectCooldownTimer > deselectCooldown)
        //             {
        //                 selectedMagic.SetActive(false);
        //                 selectedMagic = null;
        //                 deselectCooldownTimer = 0;
        //             }
        //         }
        //         else
        //         {
        //             if (deselectCooldownTimer >= 0)
        //             {
        //                 deselectCooldownTimer -= Time.deltaTime;
        //             }
        //         }
        //     }
        // }
    }
}