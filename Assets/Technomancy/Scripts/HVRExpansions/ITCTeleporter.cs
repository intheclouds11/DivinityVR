using HurricaneVR.Framework.Core.Player;

namespace intheclouds
{
    public class ITCTeleporter : HVRTeleporter
    {
        public bool playerHasEnoughAP;

        protected override bool CheckCanTeleport()
        {
            return CanTeleport && !IsTeleporting && !IsVerticalCanceling();
        }
        
        private bool IsVerticalCanceling()
        {
            if (Forward.y >= 0.8f)
            {
                CancelTeleport();
                return true;
            }

            return false;
        }

        protected override bool IsTeleportDeactivated()
        {
            if (PlayerInputs.IsTeleportDeactivated)
            {
                LocalUserObjects.Instance.genericPointerInfo.HideInfo(ActionType.Movement);
                LocalUserObjects.Instance.HUDController.ToggleTeleportCancelReminder(false);

                if (!LocalUserObjects.Instance.PlayerStats.InCombat || playerHasEnoughAP)
                {
                    return true;
                }
                else
                {
                    CancelTeleport();
                    return false;
                }
            }

            return false;
        }
        
        public void CancelTeleport()
        {
            LocalUserObjects.Instance.genericPointerInfo.MovementIcon.SetActive(false);
            LocalUserObjects.Instance.genericPointerInfo.gameObject.SetActive(false);
            
            ToggleGraphics(false);
            IsAiming = false;
        }
    }
}
