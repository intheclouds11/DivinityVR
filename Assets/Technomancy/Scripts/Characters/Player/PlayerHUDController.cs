using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class PlayerHUDController : MonoBehaviour
    {
        public TextMeshProUGUI pointerText;
        public TextMeshProUGUI headSelectionText;
        public TextMeshProUGUI pointerStatusEffectsText;
        public GameObject pointerBackground;
        public GameObject attackIcon;
        public GameObject movementIcon;
        public GameObject infoPopupParent;
        public GameObject infoPopupPrefab;
        public Transform hudFollowPoint;
        public float lerpFactor = 100f;
        public float maxSmoothDampDistance = 0.12f;

        public List<ITCPopup> HoverInfoList; // should only be two, one for each hand

        [field: SerializeField] public GameObject PointerUI { get; private set; }
        [field: SerializeField] public GameObject PointerStatusEffects { get; private set; } // contains enemy/ally status effects. Self status effects on wrist still.
        [field: SerializeField] public GameObject LeanWarning { get; private set; }
        [field: SerializeField] public GameObject TeleportCancelReminder { get; private set; }

        private Vector3 _velocity;
        private List<StatusEffect> currentPointerStatusEffects = new List<StatusEffect>();

        private void Awake()
        {
            PointerUI.SetActive(false);
            LeanWarning.SetActive(false);
            TeleportCancelReminder.SetActive(false);
            PointerStatusEffects.SetActive(false);
            headSelectionText.gameObject.SetActive(false);
            attackIcon.SetActive(false);
            movementIcon.SetActive(false);
            pointerText.text = "";
            pointerBackground.SetActive(false);
        }

        private void Update()
        {
            var distanceFromFollowPoint = Mathf.Clamp(Vector3.Distance(transform.position, hudFollowPoint.position), 0, maxSmoothDampDistance);
            var lerpTime = Time.deltaTime * lerpFactor * distanceFromFollowPoint;
            transform.position = Vector3.SmoothDamp(transform.position, hudFollowPoint.position, ref _velocity, lerpTime);
        }

        public void NewInfoPopup(string infoText, Color color)
        {
            var infoPopup = Instantiate(infoPopupPrefab, infoPopupParent.transform, false).GetComponent<ITCPopup>();
            infoPopup.TextMeshProUGUI.text = infoText;
            infoPopup.TextMeshProUGUI.color = color;
        }

        public void ToggleLeanWarning(bool setActive)
        {
            LeanWarning.SetActive(setActive);
            ToggleTeleportCancelReminder(false);
        }

        public void ToggleTeleportCancelReminder(bool setActive)
        {
            TeleportCancelReminder.SetActive(setActive);
        }

        public void ShowPointerUI(ActionType type, string text)
        {
            // todo: add heal type check. Highest activation priority

            if (type == ActionType.Attack && !movementIcon.activeSelf)
            {
                attackIcon.SetActive(true);
                pointerText.text = text;
                pointerBackground.SetActive(true);
            }
            else if (type == ActionType.Selection)
            {
                headSelectionText.text = text;
                headSelectionText.gameObject.SetActive(true);
            }
            else if (type == ActionType.Movement)
            {
                attackIcon.SetActive(false);
                movementIcon.SetActive(true);
                pointerBackground.SetActive(true);
                pointerText.text = text;
            }

            PointerUI.SetActive(true);
        }

        public void HidePointerUI(ActionType type)
        {
            //todo: add heal type check. Highest activation priority

            if (type == ActionType.Attack)
            {
                attackIcon.SetActive(false);
            }
            else if (type == ActionType.Selection)
            {
                headSelectionText.gameObject.SetActive(false);
            }
            else if (type == ActionType.Movement)
            {
                movementIcon.SetActive(false);
            }

            if (!attackIcon.activeSelf && !movementIcon.activeSelf)
            {
                pointerBackground.SetActive(false);
                pointerText.text = "";

                if (!headSelectionText.gameObject.activeSelf)
                {
                    PointerUI.SetActive(false);
                }
            }
        }

        public void ShowEnemyStatusEffectsUI(List<StatusEffect> statusEffects)
        {
            PointerStatusEffects.SetActive(true);
            bool updateText = false;

            if (currentPointerStatusEffects == null)
            {
                updateText = true;
            }
            else
            {
                foreach (var statusEffect in statusEffects)
                {
                    if (!currentPointerStatusEffects.Contains(statusEffect))
                    {
                        updateText = true;
                        break;
                    }
                }
            }

            if (updateText)
            {
                pointerStatusEffectsText.text = String.Empty;
                foreach (var statusEffect in statusEffects)
                {
                    pointerStatusEffectsText.text += $"{statusEffect.type} for {statusEffect.cooldownTimer} rounds \n";
                }
            }

            currentPointerStatusEffects = statusEffects;
        }

        public void HideEnemyStatusEffectsUI()
        {
            PointerStatusEffects.SetActive(false);
            currentPointerStatusEffects = null;
        }
    }
}