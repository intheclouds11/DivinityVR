using UnityEngine;

namespace intheclouds
{
    public class PlayerHUDController : MonoBehaviour
    {
        [SerializeField]
        private GameObject LeanWarning;
        [SerializeField]
        private GameObject TeleportCancelReminder;

        private void Awake()
        {
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
    }
}
