using HurricaneVR.Framework.Core.Player;
using UnityEngine;

namespace intheclouds
{
    public class PlayerMovementAP : MonoBehaviour
    {
        public float playerLeanThreshold = 1;
        public float APDistanceUnit = 3f;
        public GameObject LastValidPositionMarker;
        private HVRPlayerController playerController;
        private PlayerStats playerStats;
        private float APNeededForTeleport;
        private ITCTeleporter teleporter;
        private Vector3 currentPosition;

        private void OnEnable()
        {
            playerStats = LocalUserObjects.Instance.PlayerStats;
            playerController = LocalUserObjects.Instance.ITCPlayerController;
            playerController.MovementEnabled = false;
            playerController.CanCrouch = false;
            teleporter = LocalUserObjects.Instance.ITCTeleporter;
            teleporter.BeforeTeleport.AddListener(BeforeTeleport);
            teleporter.Dash = true;
            currentPosition = new Vector3(transform.position.x, 0, transform.position.z);
        }

        private void OnDisable()
        {
            playerController.MovementEnabled = true;
            playerController.CanCrouch = true;


            teleporter.Dash = false;
            teleporter.BeforeTeleport.RemoveListener(BeforeTeleport);
        }

        private void Update()
        {
            CheckLean();
            CheckTeleport();

            if (teleporter.TeleportState == TeleportState.Dashing)
            {
                currentPosition = new Vector3(transform.position.x, 0, transform.position.z);
            }
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

        private void BeforeTeleport(Vector3 arg0)
        {
            playerStats.UseAP((int) Mathf.Ceil(APNeededForTeleport));
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
                LastValidPositionMarker.SetActive(true);
                LastValidPositionMarker.transform.position = currentPosition;
            }
            else
            {
                if (!playerStats.Leaning) return;
                LocalUserObjects.Instance.HUDController.ToggleLeanWarning(false);
                LocalUserObjects.Instance.HVRPlayerInputs.UpdateInputs = true;
                playerStats.Leaning = false;
                LastValidPositionMarker.SetActive(false);
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