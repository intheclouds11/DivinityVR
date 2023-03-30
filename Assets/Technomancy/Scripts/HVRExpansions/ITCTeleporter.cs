using HurricaneVR.Framework.Core.Player;

namespace intheclouds
{
    public class ITCTeleporter : HVRTeleporter
    {
        public bool playerHasEnoughAP;

        protected override void EnabledCheck()
        {
            if (PlayerGroundedCheck && Player && !Player.IsGrounded)
            {
                Disable();
                return;
            }

            if (PlayerRotateCheck && Player && _timeSinceLastRotation < RotationTeleportThreshold && !IsAiming)
            {
                Disable();
                return;
            }

            if (PlayerClimbingCheck && Player && Player.IsClimbing)
            {
                Disable();
                return;
            }

            if (LocalUserObjects.Instance.PlayerStats.Leaning)
            {
                Disable();
                return;
            }
            
            if (Forward.y >= VerticalCancelThreshold)
            {
                if (VerticalCancelUntilDeactivateTeleport)
                {
                    verticalCanceled = true;
                }
                Disable();
                return;
            }

            if (LocalUserObjects.Instance.PlayerStats.CanPerformActions())
            {
                Enable();
            }
        }

        public override void Disable()
        {
            base.Disable();
            LocalUserObjects.Instance.genericPointerInfo.HideInfo(ActionType.Movement);
        }

        protected override bool IsTeleportDeactivated()
        {
            if (PlayerInputs.IsTeleportDeactivated)
            {
                if (VerticalCancelUntilDeactivateTeleport && Forward.y <= VerticalCancelThreshold)
                {
                    verticalCanceled = false;
                }
                
                LocalUserObjects.Instance.genericPointerInfo.HideInfo(ActionType.Movement);
                LocalUserObjects.Instance.HUDController.ToggleTeleportCancelReminder(false);

                if (!LocalUserObjects.Instance.PlayerStats.InCombat || playerHasEnoughAP && LocalUserObjects.Instance.PlayerStats.CanPerformActions())
                {
                    return true;
                }
                else
                {
                    Disable();
                    return false;
                }
            }

            return false;
        }
    }
}