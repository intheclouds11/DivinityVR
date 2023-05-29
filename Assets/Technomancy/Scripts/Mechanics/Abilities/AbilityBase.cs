using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.Utils;
using HurricaneVR.Framework.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class AbilityBase : MonoBehaviour
    {
        public enum SelectionType
        {
            None,
            Location,
            Combatant,
        }

        public int cooldown;
        public int cooldownTimer;
        [FormerlySerializedAs("amount")]
        public int baseAmount;
        public int scaledAmount;
        public int requiredAP;
        public ScalingType ScalingType;
        public SelectionType selectionType;
        public bool isOffensiveSelector;
        public GameObject abilityDescription;
        public GameObject surfaceEffect;
        public StatusEffect statusEffect;
        public GameObject activatedVFX;
        public GameObject casterVFX;
        public AudioClip activatedSFX;
        [HideInInspector]
        public AbilitySystem abilitySystem;
        [HideInInspector]
        public AbilitySlot abilitySlot;
        [HideInInspector]
        public PlayerStats caster;
        [HideInInspector]
        public HVRHandGrabber castingHand;
        protected AbilityPointer abilityPointer;

        protected virtual void OnEnable()
        {
            SelectorConfig();
        }

        protected virtual void OnDisable()
        {
            SelectorDeconfig();
        }

        private void Start()
        {
            if (baseAmount != 0)
            {
                ApplyScaling();
            }
        }

        public void OnAbilityReady()
        {
            if (activatedVFX != null)
            {
                activatedVFX.transform.parent = transform;
                activatedVFX.transform.localPosition = Vector3.zero;
                activatedVFX.transform.localRotation = Quaternion.identity;
                activatedVFX.SetActive(false);
            }
            
            abilitySlot.readyArt.SetActive(true);
            abilitySlot.cooldownArt.SetActive(false);
        }

        protected void OnAbilityUsed()
        {
            if (activatedSFX)
            {
                SFXPlayer.Instance.PlaySFX(activatedSFX, transform.position);
            }
            
            if (castingHand.Controller.Side == HVRHandSide.Left)
            {
                caster.LocalUserObjects.leftHandGrabber.ForceRelease();
            }
            else
            {
                caster.LocalUserObjects.rightHandGrabber.ForceRelease();
            }

            abilitySystem.DequipAbility();
            cooldownTimer = cooldown;
            abilitySlot.readyArt.SetActive(false);
            abilitySlot.cooldownArt.SetActive(true);
            abilitySlot.cooldownArt.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Cooldown: {cooldown}";
        }

        protected void ResetAbilityTransform()
        {
            gameObject.SetActive(false);
            transform.parent = caster.LocalUserObjects.abilities.transform;
            transform.position = caster.LocalUserObjects.abilities.transform.position;
            transform.rotation = caster.LocalUserObjects.abilities.transform.rotation;
        }

        public void ApplyScaling()
        {
            if (ScalingType == ScalingType.Pyrokinetic)
            {
                scaledAmount = baseAmount * abilitySystem.playerLUOs.PlayerStats.level * (1 + abilitySystem.playerLUOs.PlayerStats.Pyrokinetic);
            }
            else if (ScalingType == ScalingType.Hydrosophist)
            {
                scaledAmount = baseAmount * abilitySystem.playerLUOs.PlayerStats.level * (1 + abilitySystem.playerLUOs.PlayerStats.Hydrosophist);
            }
            else if (ScalingType == ScalingType.Geomancer)
            {
                scaledAmount = baseAmount * abilitySystem.playerLUOs.PlayerStats.level * (1 + abilitySystem.playerLUOs.PlayerStats.Geomancer);
            }

            Debug.Log($"updated {name} amount based on player stats");
        }

        private void SelectorConfig()
        {
            if (selectionType == SelectionType.Location)
            {
                if (castingHand.Controller.Side == HVRHandSide.Left)
                {
                    AbilitySpawnLocator.instance.SelectionLineSource = AbilitySpawnLocator.instance.SelectionLineSourceRight;
                }
                else
                {
                    AbilitySpawnLocator.instance.SelectionLineSource = AbilitySpawnLocator.instance.SelectionLineSourceLeft;
                }

                AbilitySpawnLocator.instance.enabled = true;
            }
            else if (selectionType == SelectionType.Combatant)
            {
                if (castingHand.Controller.Side == HVRHandSide.Left)
                {
                    abilityPointer = caster.LocalUserObjects.rightAbilityPointer;
                }
                else
                {
                    abilityPointer = caster.LocalUserObjects.leftAbilityPointer;
                }

                abilityPointer.isOffensiveHighlight = isOffensiveSelector;
                abilityPointer.gameObject.SetActive(true);
            }
        }

        private void SelectorDeconfig()
        {
            if (selectionType == SelectionType.Location)
            {
                AbilitySpawnLocator.instance.enabled = false;
            }
            else if (selectionType == SelectionType.Combatant)
            {
                if (abilityPointer.combatantSelected)
                {
                    abilityPointer.combatantSelected.modelHighlightEffect.highlighted = false;
                }

                abilityPointer.gameObject.SetActive(false);
            }
        }
    }
}