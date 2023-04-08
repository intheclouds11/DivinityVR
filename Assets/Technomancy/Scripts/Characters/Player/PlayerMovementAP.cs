using HurricaneVR.Framework.Core.Player;
using UnityEngine;

namespace intheclouds
{
    public class PlayerMovementAP : MonoBehaviour
    {
        public float playerLeanThreshold = 1;
        public float APDistanceUnit = 3f;
        private HVRPlayerController playerController;
        private PlayerStats playerStats;
        private float APNeededForTeleport;
        private ITCTeleporter teleporter;
        private Vector3 currentPosition;

        private void OnEnable()
        {
            playerStats = LocalUserObjects.Instance.PlayerStats;
            playerController = LocalUserObjects.Instance.HVRPlayerController;
            playerController.MovementEnabled = false;
            teleporter = LocalUserObjects.Instance.ITCTeleporter;
            teleporter.BeforeTeleportAction += OnTeleport;
            teleporter.Dash = true;
            currentPosition = transform.position;
        }

        private void OnDisable()
        {
            if (playerController)
            {
                playerController.MovementEnabled = true;
            }
            
            teleporter.Dash = false;
            teleporter.BeforeTeleportAction -= OnTeleport;
        }

        private void Update()
        {
            CheckLean();
            CheckTeleport();
        }

        private void CheckTeleport()
        {
            if (!playerStats.CanPerformActions()) return;

            if (teleporter.IsAiming)
            {
                LocalUserObjects.Instance.HUDController.ToggleTeleportCancelReminder(true);
                APNeededForTeleport = teleporter.teleportPathLength / APDistanceUnit;
                LocalUserObjects.Instance.HUDController.ShowPointerUI(ActionType.Movement, $"Teleport AP: {(int) Mathf.Ceil(APNeededForTeleport)}");
                
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
            var distance = Vector3.Distance(playerController.transform.position, currentPosition);

            if (distance > playerLeanThreshold)
            {
                if (playerStats.Leaning) return;
                LocalUserObjects.Instance.HUDController.ToggleLeanWarning(true);
                LocalUserObjects.Instance.HVRPlayerInputs.UpdateInputs = false;
                playerStats.Leaning = true;
            }
            else
            {
                if (!playerStats.Leaning) return;
                LocalUserObjects.Instance.HUDController.ToggleLeanWarning(false);
                LocalUserObjects.Instance.HVRPlayerInputs.UpdateInputs = true;
                playerStats.Leaning = false;
            }
        }

        public void StartTurn()
        {
        }

        public void EndTurn()
        {
        }
    }
}