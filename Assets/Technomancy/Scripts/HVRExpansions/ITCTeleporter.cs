using System.Collections.Generic;
using HurricaneVR.Framework.Core.Player;
using Pathfinding;
using UnityEngine;

namespace intheclouds
{
    public class ITCTeleporter : HVRTeleporter
    {
        public bool playerHasEnoughAP;
        public float teleportPathLength { get; private set; }
        private Seeker _seeker;
        private LocalUserObjects _localUserObjects;
        private List<Vector3> _teleportVectorPath;
        private Coroutine _dashCoroutine;
        private float _previousJoystickX;
        private float _previousJoystickY;

        protected override void Awake()
        {
            base.Awake();
            _localUserObjects = LocalUserObjects.instance;
            _seeker = GetComponent<Seeker>();
            _seeker.pathCallback += OnPathComplete;
        }

        private void OnDisable()
        {
            _seeker.pathCallback -= OnPathComplete;
        }

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

            if (LocalUserObjects.instance.PlayerStats.Leaning)
            {
                Disable();
                return;
            }

            if (Forward.y * 90 >= VerticalCancelAngle)
            {
                if (VerticalCancelUntilDeactivateTeleport)
                {
                    verticalCanceled = true;
                }

                Disable();
                return;
            }

            if (LocalUserObjects.instance.PlayerStats.CanPerformActions())
            {
                Enable();
            }
        }

        public override void Disable()
        {
            base.Disable();
            LocalUserObjects.instance.HUDController.HidePointerUI(ActionType.Movement);
        }

        protected override void OnTeleportActivated()
        {
            base.OnTeleportActivated();
            RotationModifier = 0;
        }

        protected override bool IsTeleportDeactivated()
        {
            if (PlayerInputs.IsTeleportDeactivated)
            {
                if (VerticalCancelUntilDeactivateTeleport && Forward.y * 90 <= VerticalCancelAngle)
                {
                    verticalCanceled = false;
                }

                LocalUserObjects.instance.HUDController.HidePointerUI(ActionType.Movement);
                LocalUserObjects.instance.HUDController.ToggleTeleportCancelReminder(false);

                if (!LocalUserObjects.instance.PlayerStats.InCombat || playerHasEnoughAP && LocalUserObjects.instance.PlayerStats.CanPerformActions())
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

        protected override bool CheckValidDestination(GameObject hitObject, Vector3 destination, Vector3 surfaceNormal)
        {
            if (!CheckDestinationAllowed(hitObject, destination))
                return false;

            if (!CheckDestinationLineOfSight(destination))
                return false;

            if (!CheckPlayerFits(destination))
                return false;

            if (!CheckSurfaceAngle(surfaceNormal))
                return false;

            if (!CheckVerticalDistance(destination))
                return false;

            if (Dash && _localUserObjects.PlayerStats.CanPerformActions() && _seeker.IsDone())
            {
                var path = _seeker.StartPath(transform.position, TeleportDestination);
            }

            if (RotateTeleportDestination)
            {
                CheckDestinationRotation();
            }

            return true;
        }

        private void OnPathComplete(Path p)
        {
            if (p.error)
            {
                // Debug.Log($"ITCTeleporter.OnPathComplete(): a valid path can't be found");
            }
            else
            {
                teleportPathLength = 0;
                _teleportVectorPath = p.vectorPath;
                List<Vector3> visualPath = new List<Vector3>();
                foreach (var point in p.vectorPath)
                {
                    var adjustedPoint = point;
                    adjustedPoint.y += 0.25f;
                    visualPath.Add(adjustedPoint);
                }

                TeleportPath.positionCount = visualPath.Count;
                TeleportPath.SetPositions(visualPath.ToArray());
                for (var i = 0; i < p.vectorPath.Count - 1; i++)
                {
                    teleportPathLength += Vector3.Distance(p.vectorPath[i], p.vectorPath[i + 1]);
                }
            }
        }
        
        private void CheckDestinationRotation()
        {
            if (PlayerInputs.TurnAxis.magnitude >= 0.95f)
            {
                RotationModifier = PlayerInputs.TurnAxis.x * RotateTeleportAmount;
            }

            TeleportMarker.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + RotationModifier, 0);
        }

        protected override void UpdateDashTeleport()
        {
            if (_teleportVectorPath.Count > 0)
            {
                if (Vector3.Distance(FeetPosition, _teleportVectorPath[0]) > .01)
                {
                    UpdatePlayerPositionAndRotation(Vector3.MoveTowards(FeetPosition, _teleportVectorPath[0], DashSpeed * Time.deltaTime), 0);
                    PositionUpdate.Invoke(FeetPosition);
                }
                else
                {
                    UpdatePlayerPositionAndRotation(_teleportVectorPath[0], 0);
                    PositionUpdate.Invoke(FeetPosition);
                    _teleportVectorPath.RemoveAt(0);
                }
            }
            else
            {
                UpdatePlayerPositionAndRotation(TeleportDestination, RotationModifier);
                TeleportState = TeleportState.AwaitingNextFrame;
            }
        }

        protected override void OnBeforeDashTeleport()
        {
            if (_localUserObjects.ITCPlayerController.TunnelTeleport)
            {
                _localUserObjects.ITCPlayerController.TunnellingMobile.useVelocity = true;
            }
            base.OnBeforeDashTeleport();
        }

        protected override void OnAfterDashTeleport()
        {
            if (_localUserObjects.ITCPlayerController.TunnelTeleport)
            {
                _localUserObjects.ITCPlayerController.TunnellingMobile.useVelocity = false;
            }
            base.OnAfterDashTeleport();
        }
        
        protected override void OnBeforeTeleport()
        {
            if (_localUserObjects.ITCPlayerController.TunnelMovementInput)
            {
                _localUserObjects.ITCPlayerController.TunnellingMobile.useVelocity = false;
            }
            base.OnBeforeTeleport();
        }

        protected override void OnAfterTeleport()
        {
            if (_localUserObjects.ITCPlayerController.TunnelMovementInput)
            {
                _localUserObjects.ITCPlayerController.TunnellingMobile.useVelocity = true;
            }
            base.OnAfterTeleport();
        }
    }
}