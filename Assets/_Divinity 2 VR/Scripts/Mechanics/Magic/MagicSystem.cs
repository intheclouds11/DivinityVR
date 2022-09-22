using System;
using HighlightPlus;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.Utils;
using HurricaneVR.Framework.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class MagicSystem : MonoBehaviour
    {
        public GameObject magicSlots;
        public Magic selectedMagic;
        public GameObject spawnedMagic;
        public GameObject description;
        public LocalUserObjects playerLUOs;
        public HVRHandGrabber Grabber { get; set; }
        public HVRGrabbable Grabbable;
        private HVRHandGrabber leftHandGrabber;
        private HVRHandGrabber rightHandGrabber;
        private HVRController leftController;
        private HVRController rightController;
        private HVRController selectorHand;
        private float cooldownTimerNoCombat;


        private void Awake()
        {
            playerLUOs = transform.root.GetComponent<LocalUserObjects>();
            leftHandGrabber = playerLUOs.leftHandPhysics.GetComponent<HVRHandGrabber>();
            rightHandGrabber = playerLUOs.rightHandPhysics.GetComponent<HVRHandGrabber>();

            if (magicSlots.activeInHierarchy)
            {
                magicSlots.SetActive(false);
            }
        }

        private void Start()
        {
            leftController = playerLUOs.HVRPlayerInputs.LeftController;
            rightController = playerLUOs.HVRPlayerInputs.RightController;
        }

        void Update()
        {
            SelectorUpdate();

            if (!playerLUOs.PlayerStats.InCombat)
            {
                CooldownExploration();
            }

            if (selectedMagic)
            {
                CheckMagicActivation();
            }
        }

        private void SelectorUpdate()
        {
            if (leftController.TrackpadButtonState.JustActivated && !magicSlots.activeSelf)
            {
                ShowSelector(playerLUOs.leftHandMagicSelectorSpawn.transform, leftController);
            }
            else if (rightController.TrackpadButtonState.JustActivated && !magicSlots.activeSelf)
            {
                ShowSelector(playerLUOs.rightHandMagicSelectorSpawn.transform, rightController);
            }

            if (!magicSlots.activeSelf)
            {
                return;
            }

            if (selectorHand == leftController && leftController.TrackpadButtonState.JustDeactivated)
            {
                HideSelector();
            }
            else if (selectorHand == rightController && rightController.TrackpadButtonState.JustDeactivated)
            {
                HideSelector();
            }
        }

        public void ShowSelector(Transform spawnPoint, HVRController hand)
        {
            selectorHand = hand;
            transform.position = spawnPoint.position;
            var newEulerAngles = spawnPoint.eulerAngles;
            newEulerAngles = new Vector3(30, newEulerAngles.y, 0);
            transform.eulerAngles = newEulerAngles;

            magicSlots.SetActive(true);
            if (description.transform.childCount > 0)
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

        private void CheckMagicActivation()
        {
            if (!spawnedMagic && selectedMagic.cooldownTimer == 0)
            {
                if (leftController.TriggerButtonState.JustActivated && leftController.GripButtonState.Active &&
                    !leftHandGrabber.TriggerHoverTarget && !leftHandGrabber.IsGrabbing)
                {
                    SpawnMagic(playerLUOs.HVRPlayerInputs.LeftController);
                }
                else if (rightController.TriggerButtonState.JustActivated && rightController.GripButtonState.Active &&
                         !rightHandGrabber.TriggerHoverTarget && !rightHandGrabber.IsGrabbing)
                {
                    SpawnMagic(playerLUOs.HVRPlayerInputs.RightController);
                }
            }
        }

        private void SpawnMagic(HVRController controller)
        {
            if (controller == leftController)
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
            spawnedMagic.GetComponent<Magic>().caster = playerLUOs.PlayerStats;
            Grabbable = spawnedMagic.GetComponent<HVRGrabbable>();
            Grabber.TryGrab(Grabbable);
        }

        public void DequipMagic()
        {
            // Potentially more performant, but more annoying to keep descriptions accurate
            // foreach (Transform child in description.transform)
            // {
            //     if (child.name == selectedMagic.abilityDescription.name + "(Clone)")
            //     {
            //         child.gameObject.SetActive(false);
            //     }
            // }

            selectedMagic.magicSlot.readyArt.GetComponent<HighlightEffect>().highlighted = false;
            selectedMagic = null;
            foreach (Transform child in description.transform)
            {
                Destroy(child.gameObject);
            }

            var augmentHighlight = playerLUOs.handAugmentHighlight;
            augmentHighlight.overlayColor = playerLUOs.PlayerStats.statsSO.baseHandAugmentColor;
            augmentHighlight.SetGlowColor(playerLUOs.PlayerStats.statsSO.baseHandAugmentColor);
        }

        private void CooldownExploration()
        {
            if (cooldownTimerNoCombat < 2)
            {
                cooldownTimerNoCombat += Time.deltaTime;
            }
            else if (cooldownTimerNoCombat >= 2)
            {
                Cooldown();
                cooldownTimerNoCombat = 0;
            }
        }

        public void Cooldown()
        {
            foreach (Transform slotGO in magicSlots.transform)
            {
                var slot = slotGO.GetComponent<MagicSlot>();
                if (slot.magic == null)
                {
                    return;
                }

                if (slot.magic.cooldownTimer > 0)
                {
                    slot.magic.cooldownTimer -= 1;
                    slot.cooldownArt.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Cooldown: {slot.magic.cooldownTimer}";
                }

                if (slot.magic.cooldownTimer == 0 && !slot.readyArt.activeSelf)
                {
                    slot.magic.OnMagicReady();
                }
            }
        }
    }
}