using HurricaneVR.Framework.Core.Player;

namespace intheclouds
{
    public class ITCTeleporter : HVRTeleporter
    {
        public bool playerHasEnoughAP;

        protected override bool CheckCanTeleport()
        {
            return !IsVerticalCanceling() && CanTeleport && !IsTeleporting && LocalUserObjects.Instance.PlayerStats.CanPerformActions();
        }

        private bool IsVerticalCanceling()
        {
            if (Forward.y >= 0.8f && CanTeleport)
            {
                CancelTeleport();
                return true;
            }

            if (Forward.y <= 0.8f)
            {
                CanTeleport = true;
            }

            return false;
        }

        protected override bool IsTeleportDeactivated()
        {
            if (PlayerInputs.IsTeleportDeactivated)
            {
                LocalUserObjects.Instance.genericPointerInfo.HideInfo(ActionType.Movement);
                LocalUserObjects.Instance.HUDController.ToggleTeleportCancelReminder(false);

                if (!LocalUserObjects.Instance.PlayerStats.InCombat || playerHasEnoughAP && LocalUserObjects.Instance.PlayerStats.CanPerformActions())
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
            CanTeleport = false;
        }
    }
}