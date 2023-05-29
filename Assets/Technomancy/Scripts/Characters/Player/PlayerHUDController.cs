using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class PlayerHUDController : MonoBehaviour
    {
        public TextMeshProUGUI PointerText;
        public TextMeshProUGUI HeadSelectionText;
        public GameObject PointerBackground;
        public GameObject AttackIcon;
        public GameObject MovementIcon;
        public GameObject infoPopupParent;
        public GameObject infoPopupPrefab;

        public List<ITCPopup> HoverInfoList; // should only be two, one for each hand

        [SerializeField]
        private GameObject PointerUI;
        [SerializeField]
        private GameObject PointerStatusEffects; // contains enemy/ally status effects. Self status effects on wrist still.
        [SerializeField]
        private GameObject LeanWarning;
        [SerializeField]
        private GameObject TeleportCancelReminder;

        private void Awake()
        {
            PointerUI.SetActive(false);
            LeanWarning.SetActive(false);
            TeleportCancelReminder.SetActive(false);
            HeadSelectionText.gameObject.SetActive(false);
            AttackIcon.SetActive(false);
            MovementIcon.SetActive(false);
            PointerText.text = "";
            PointerBackground.SetActive(false);
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
        }
        
        public void ToggleTeleportCancelReminder(bool setActive)
        {
            TeleportCancelReminder.SetActive(setActive);
        }

        public void ShowPointerUI(ActionType type, string text)
        {
            // todo: add heal type check. Highest activation priority
            
            if (type == ActionType.Attack && !MovementIcon.activeSelf)
            {
                AttackIcon.SetActive(true);
                PointerText.text = text;
                PointerBackground.SetActive(true);
            }
            else if (type == ActionType.Selection)
            {
                HeadSelectionText.text = text;
                HeadSelectionText.gameObject.SetActive(true);
            }
            else if (type == ActionType.Movement)
            {
                AttackIcon.SetActive(false);
                MovementIcon.SetActive(true);
                PointerBackground.SetActive(true);
                PointerText.text = text;
            }
        
            PointerUI.SetActive(true);
        }

        public void HidePointerUI(ActionType type)
        {
            //todo: add heal type check. Highest activation priority

            if (type == ActionType.Attack)
            {
                AttackIcon.SetActive(false);
            }
            else if (type == ActionType.Selection)
            {
                HeadSelectionText.gameObject.SetActive(false);
            }
            else if (type == ActionType.Movement)
            {
                MovementIcon.SetActive(false);
            }

            if (!AttackIcon.activeSelf && !MovementIcon.activeSelf)
            {
                PointerBackground.SetActive(false);
                PointerText.text = "";

                if (!HeadSelectionText.gameObject.activeSelf)
                {
                    PointerUI.SetActive(false);
                }
            }
        }
    }
}
