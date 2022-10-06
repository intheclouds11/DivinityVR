using System;
using HighlightPlus;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class AbilitySystem : MonoBehaviour
    {
        public GameObject abilitySlots;
        public AbilityBase selectedAbility;
        public GameObject description;
        private HVRHandGrabber leftHandGrabber;
        private HVRHandGrabber rightHandGrabber;
        private HVRController leftController;
        private HVRController rightController;
        private HVRController selectorHand;
        private float cooldownTimerNoCombat;
        [Header("Debug")]
        public LocalUserObjects playerLUOs;
        public HVRHandGrabber Grabber { get; set; }
        public HVRGrabbable Grabbable;


        private void Awake()
        {
            playerLUOs = transform.root.GetComponent<LocalUserObjects>();
            leftHandGrabber = playerLUOs.leftHandPhysics.GetComponent<HVRHandGrabber>();
            rightHandGrabber = playerLUOs.rightHandPhysics.GetComponent<HVRHandGrabber>();

            if (abilitySlots.activeInHierarchy)
            {
                abilitySlots.SetActive(false);
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

            CheckAbilityEnable();
        }

        private void SelectorUpdate()
        {
            if (!selectedAbility || selectedAbility && !selectedAbility.gameObject.activeInHierarchy)
            {
                if (leftController.TrackpadButtonState.JustActivated && !abilitySlots.activeSelf)
                {
                    ShowSelector(playerLUOs.leftHandAbilitySelectorSpawn.transform, leftController);
                }
                else if (rightController.TrackpadButtonState.JustActivated && !abilitySlots.activeSelf)
                {
                    ShowSelector(playerLUOs.rightHandAbilitySelectorSpawn.transform, rightController);
                }
            }

            if (!abilitySlots.activeSelf)
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

            abilitySlots.SetActive(true);
            if (description.transform.childCount > 0)
            {
                description.SetActive(true);
            }

            playerLUOs.handAugmentHighlight.highlighted = true;
        }

        public void HideSelector()
        {
            if (!selectedAbility)
            {
                playerLUOs.handAugmentHighlight.overlayColor = playerLUOs.PlayerStats.statsSO.baseHandAugmentColor;
                playerLUOs.handAugmentHighlight.SetGlowColor(playerLUOs.PlayerStats.statsSO.baseHandAugmentColor);
                playerLUOs.handAugmentHighlight.highlighted = false;
            }

            abilitySlots.SetActive(false);
            if (description.transform.childCount == 1)
            {
                description.SetActive(false);
            }
        }

        private void CheckAbilityEnable()
        {
            if ((playerLUOs.PlayerStats.Turn || !playerLUOs.PlayerStats.InCombat) && selectedAbility != null && !selectedAbility.gameObject.activeInHierarchy &&
                selectedAbility.cooldownTimer == 0 && playerLUOs.PlayerStats.CurrentAP >= selectedAbility.requiredAP && !playerLUOs.spiritWander.isActivated)
            {
                if (leftController.TriggerButtonState.JustActivated && leftController.GripButtonState.Active &&
                    !leftHandGrabber.TriggerHoverTarget && !leftHandGrabber.IsGrabbing)
                {
                    EnableAbility(playerLUOs.HVRPlayerInputs.LeftController);
                }
                else if (rightController.TriggerButtonState.JustActivated && rightController.GripButtonState.Active &&
                         !rightHandGrabber.TriggerHoverTarget && !rightHandGrabber.IsGrabbing)
                {
                    EnableAbility(playerLUOs.HVRPlayerInputs.RightController);
                }
            }
        }

        private void EnableAbility(HVRController controller)
        {
            if (controller == leftController)
            {
                selectedAbility.transform.position = playerLUOs.leftHandPalm.transform.position;
                Grabber = playerLUOs.leftHandPhysics.GetComponent<HVRHandGrabber>();
            }
            else
            {
                selectedAbility.transform.position = playerLUOs.rightHandPalm.transform.position;
                Grabber = playerLUOs.rightHandPhysics.GetComponent<HVRHandGrabber>();
            }

            if (playerLUOs.PlayerStats.InCombat)
            {
                playerLUOs.PlayerStats.UseAP(selectedAbility.requiredAP);
            }

            selectedAbility.castingHand = controller.Side;
            selectedAbility.caster = playerLUOs.PlayerStats;
            selectedAbility.gameObject.SetActive(true);
            selectedAbility.enabled = true;
            Grabbable = selectedAbility.GetComponent<HVRGrabbable>();
            Grabber.TryGrab(Grabbable);
        }

        public void DequipAbility()
        {
            selectedAbility.abilitySlot.readyArt.GetComponent<HighlightEffect>().highlighted = false;
            selectedAbility = null;
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
            foreach (Transform slotGO in abilitySlots.transform)
            {
                var slot = slotGO.GetComponent<AbilitySlot>();
                if (slot.ability == null)
                {
                    continue;
                }

                if (slot.ability.cooldownTimer > 0)
                {
                    slot.ability.cooldownTimer -= 1;
                    slot.cooldownArt.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Cooldown: {slot.ability.cooldownTimer}";
                }

                if (slot.ability.cooldownTimer == 0 && !slot.readyArt.activeSelf)
                {
                    slot.ability.OnAbilityReady();
                }
            }
        }
    }
}