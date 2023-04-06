using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HurricaneVR.Framework.Core.Player;
using Pathfinding;
using UnityEngine;

namespace intheclouds
{
    public class ITCTeleporter : HVRTeleporter
    {
        public bool playerHasEnoughAP;
        public float teleportPathLength { get; private set; }
        private AIDestinationSetter aiDestinationSetter;
        private Seeker seeker;
        private LocalUserObjects localUserObjects;
        private List<Vector3> teleportVectorPath;
        private Coroutine dashCoroutine;

        protected override void Awake()
        {
            base.Awake();
            localUserObjects = LocalUserObjects.Instance;
            aiDestinationSetter = GetComponent<AIDestinationSetter>();
            seeker = GetComponent<Seeker>();
            seeker.pathCallback += OnPathComplete;
        }

        private void OnDisable()
        {
            seeker.pathCallback -= OnPathComplete;
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
            LocalUserObjects.Instance.HUDController.HidePointerUI(ActionType.Movement);
        }

        protected override bool IsTeleportDeactivated()
        {
            if (PlayerInputs.IsTeleportDeactivated)
            {
                if (VerticalCancelUntilDeactivateTeleport && Forward.y <= VerticalCancelThreshold)
                {
                    verticalCanceled = false;
                }
                
                LocalUserObjects.Instance.HUDController.HidePointerUI(ActionType.Movement);
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

            if (localUserObjects.PlayerStats.CanPerformActions() && seeker.IsDone())
            {
                var path = seeker.StartPath(transform.position, TeleportDestination);
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
                teleportVectorPath = p.vectorPath;
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
        
        protected override void UpdateDashTeleport()
        {
            if (teleportVectorPath.Count > 0)
            {
                if (Vector3.Distance(FeetPosition, teleportVectorPath[0]) > .01)
                {
                    UpdatePlayerPosition(Vector3.MoveTowards(FeetPosition, teleportVectorPath[0], DashSpeed * Time.deltaTime));
                    PositionUpdate.Invoke(FeetPosition);
                }
                else
                {
                    UpdatePlayerPosition(teleportVectorPath[0]);
                    PositionUpdate.Invoke(FeetPosition);
                    teleportVectorPath.RemoveAt(0);
                }
            }
            else
            {
                TeleportState = TeleportState.AwaitingNextFrame;
            }
        }
    }
}