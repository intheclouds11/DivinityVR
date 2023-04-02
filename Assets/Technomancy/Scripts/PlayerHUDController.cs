using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class PlayerHUDController : MonoBehaviour
    {
        public TextMeshProUGUI PointerText;
        public GameObject AttackIcon;
        public GameObject MovementIcon;

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
            }
            else if (type == ActionType.Movement)
            {
                AttackIcon.SetActive(false);
                MovementIcon.SetActive(true);
                PointerText.text = text;
            }
        
            PointerUI.SetActive(true);
        }

        public void HidePointerUI(ActionType infoType)
        {
            //todo: add heal type check. Highest activation priority

            if (infoType == ActionType.Attack)
            {
                AttackIcon.SetActive(false);
            }
            else if (infoType == ActionType.Movement)
            {
                MovementIcon.SetActive(false);
            }

            if (!AttackIcon.activeSelf && !MovementIcon.activeSelf)
            {
                PointerUI.SetActive(false);
            }
        }
    }
}
