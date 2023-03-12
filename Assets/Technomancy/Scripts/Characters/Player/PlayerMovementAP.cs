using HurricaneVR.Framework.Core.Player;
using UnityEngine;

namespace intheclouds
{
    public class PlayerMovementAP : MonoBehaviour
    {
        private HVRPlayerController playerController;
        private Vector3 previousPosition;
        private PlayerStats playerStats;
        private float distanceMoved;

        private void OnEnable()
        {
            var localUserObjects = transform.GetComponentInParent<LocalUserObjects>();
            playerStats = localUserObjects.PlayerStats;
            playerController = localUserObjects.HVRPlayerController;
            playerController.MovementEnabled = false;
        }

        private void OnDisable()
        {
            if (playerController)
            {
                playerController.MovementEnabled = true;
            }
        }

        private void Update()
        {
            if (!playerStats.Turn || playerStats.LocalUserObjects.spiritWander.isActivated) return;
            if (transform.position != previousPosition)
            {
                TrackMovementApUsage();
                // distanceMovedText.text = $"distance moved: {(int) distanceMoved}";
            }
        }

        public void StartTurn()
        {
            previousPosition = playerController.transform.position;
            distanceMoved = 0;
            playerController.MovementEnabled = true;
        }

        public void EndTurn()
        {
            playerController.MovementEnabled = false;
        }

        private void TrackMovementApUsage()
        {
            distanceMoved += Vector3.Distance(playerController.transform.position, previousPosition);

            if (distanceMoved > 3)
            {
                playerStats.UseAP(1);
                distanceMoved -= 3;
            }

            previousPosition = playerController.transform.position;
        }
    }
}