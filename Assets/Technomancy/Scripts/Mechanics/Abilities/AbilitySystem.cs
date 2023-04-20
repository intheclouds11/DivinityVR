using System.Collections;
using HighlightPlus;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Shared;
using TMPro;
using UnityEngine;

namespace intheclouds
{
    public class AbilitySystem : MonoBehaviour
    {
        public GameObject abilitySlots;
        public AbilityBase selectedAbility;
        public GameObject description;
        public HVRHandGrabber leftHandGrabber;
        public HVRHandGrabber rightHandGrabber;
        private HVRController leftController;
        private HVRController rightController;
        private HVRController selectorHand;
        private float cooldownTimerNoCombat;
        [Header("Debug")]
        public LocalUserObjects playerLUOs;
        public HVRGrabbable Grabbable;


        private void Awake()
        {
            playerLUOs = transform.GetComponentInParent<LocalUserObjects>();
            leftHandGrabber = playerLUOs.leftHandGrabber;
            rightHandGrabber = playerLUOs.rightHandGrabber;

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
                AbilityCooldownExploration();
            }

            if (selectedAbility)
            {
                CheckAbilityEnable();
            }
        }

        private void SelectorUpdate()
        {
            if (!abilitySlots.activeSelf && (!selectedAbility || selectedAbility && !selectedAbility.gameObject.activeInHierarchy))
            {
                if (!playerLUOs.leftHandGrabber.IsGrabbing && playerLUOs.HVRPlayerInputs.isLeftAbilitySelectorActive)
                {
                    ShowSelector(playerLUOs.leftHandAbilitySelectorSpawn.transform, leftController);
                }
                else if (!playerLUOs.rightHandGrabber.IsGrabbing && playerLUOs.HVRPlayerInputs.isRightAbilitySelectorActive)
                {
                    ShowSelector(playerLUOs.rightHandAbilitySelectorSpawn.transform, rightController);
                }

                return;
            }

            if (selectorHand == leftController && !playerLUOs.HVRPlayerInputs.isLeftAbilitySelectorActive)
            {
                HideSelector();
            }
            else if (selectorHand == rightController && !playerLUOs.HVRPlayerInputs.isRightAbilitySelectorActive)
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
            if (playerLUOs.PlayerStats.CanPerformActions() && !selectedAbility.gameObject.activeInHierarchy &&
                selectedAbility.cooldownTimer == 0 && playerLUOs.PlayerStats.CurrentAP >= selectedAbility.requiredAP)
            {
                if (leftController.TriggerButtonState.Active && leftController.GripButtonState.Active &&
                    !leftHandGrabber.TriggerHoverTarget && !leftHandGrabber.IsGrabbing)
                {
                    StartCoroutine(EnableAbility(leftHandGrabber));
                }
                else if (rightController.TriggerButtonState.Active && rightController.GripButtonState.Active &&
                         !rightHandGrabber.TriggerHoverTarget && !rightHandGrabber.IsGrabbing)
                {
                    StartCoroutine(EnableAbility(rightHandGrabber));
                }
            }
        }

        private IEnumerator EnableAbility(HVRHandGrabber hand)
        {
            if (playerLUOs.PlayerStats.InCombat)
            {
                playerLUOs.PlayerStats.UseAP(selectedAbility.requiredAP);
            }

            selectedAbility.enabled = true;
            selectedAbility.castingHand = hand;
            selectedAbility.caster = playerLUOs.PlayerStats;
            selectedAbility.gameObject.SetActive(true);

            
            if (selectedAbility.TryGetComponent(out HVRGrabbable grabbable))
            {
                if (hand.Controller == leftController)
                {
                    selectedAbility.transform.position = playerLUOs.leftHandPalm.transform.position;
                    yield return null; // wait one frame so components can initialize
                    leftHandGrabber.Grab(grabbable, HVRGrabTrigger.Toggle);
                }
                else if (hand.Controller == rightController)
                {
                    selectedAbility.transform.position = playerLUOs.rightHandPalm.transform.position;
                    yield return null; // wait one frame so components can initialize
                    rightHandGrabber.Grab(grabbable, HVRGrabTrigger.Toggle);
                }
            }
            else
            {
                if (hand.Controller == leftController)
                {
                    selectedAbility.transform.parent = playerLUOs.leftHandPalm.transform;
                }
                else if (hand.Controller == rightController)
                {
                    selectedAbility.transform.parent = playerLUOs.rightHandPalm.transform;
                }

                selectedAbility.transform.localPosition = Vector3.zero;
                selectedAbility.transform.localRotation = Quaternion.identity;
            }
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

        private void AbilityCooldownExploration()
        {
            if (cooldownTimerNoCombat < 2)
            {
                cooldownTimerNoCombat += Time.deltaTime;
            }
            else if (cooldownTimerNoCombat >= 2)
            {
                AbilityCooldown();
                cooldownTimerNoCombat = 0;
            }
        }

        public void AbilityCooldown()
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