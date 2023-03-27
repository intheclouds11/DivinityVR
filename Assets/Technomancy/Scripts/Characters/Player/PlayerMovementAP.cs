using HurricaneVR.Framework.Core.Player;
using UnityEngine;

namespace intheclouds
{
    public class PlayerMovementAP : MonoBehaviour
    {
        public float playerLeanThreshold = 1;
        public float APDistanceUnit = 3f;
        private HVRPlayerController playerController;
        private Vector3 previousPosition;
        private PlayerStats playerStats;
        private float APNeededForTeleport;
        private float distanceMoved;
        private ITCTeleporter teleporter;
        private Vector3 currentPosition;

        private void OnEnable()
        {
            playerStats = LocalUserObjects.Instance.PlayerStats;
            playerController = LocalUserObjects.Instance.HVRPlayerController;
            playerController.MovementEnabled = false;
            teleporter = LocalUserObjects.Instance.ITCTeleporter;
            teleporter.BeforeTeleportAction += OnTeleport;
        }

        private void OnDisable()
        {
            if (playerController)
            {
                playerController.MovementEnabled = true;
            }
            
            teleporter.BeforeTeleportAction -= OnTeleport;
        }

        private void Update()
        {
            if (currentPosition != Vector3.zero)
            {
                CheckLean();
            }
            
            CheckTeleport();
        }

        private void CheckTeleport()
        {
            if (!playerStats.Turn || playerStats.LocalUserObjects.spiritWander.isActivated) return;

            if (teleporter.IsAiming)
            {
                LocalUserObjects.Instance.HUDController.ToggleTeleportCancelReminder(true);
                var teleportDistance = Vector3.Distance(teleporter.TeleportDestination, transform.position);
                APNeededForTeleport = teleportDistance / APDistanceUnit;
                LocalUserObjects.Instance.genericPointerInfo.ShowInfo(ActionType.Movement, $"Teleport AP: {(int) Mathf.Ceil(APNeededForTeleport)}");
                
                if (playerStats.CurrentAP >= APNeededForTeleport)
                {
                    teleporter.playerHasEnoughAP = true;
                }
                else
                {
                    teleporter.playerHasEnoughAP = false;
                }
            }
        }

        private void OnTeleport()
        {
            playerStats.UseAP((int) Mathf.Ceil(APNeededForTeleport));
            currentPosition = teleporter.TeleportDestination;
        }

        private void CheckLean()
        {
            var distance = playerController.transform.position - currentPosition;

            if (distance.magnitude > playerLeanThreshold)
            {
                LocalUserObjects.Instance.HUDController.ToggleLeanWarning(true);
                playerStats.Leaning = true;
            }
            else
            {
                LocalUserObjects.Instance.HUDController.ToggleLeanWarning(false);
                playerStats.Leaning = false;
            }
        }

        public void StartTurn()
        {
            previousPosition = playerController.transform.position;
            distanceMoved = 0;
            // playerController.MovementEnabled = true;
        }

        public void EndTurn()
        {
            // playerController.MovementEnabled = false;
        }
    }
}