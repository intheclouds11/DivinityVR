using System;
using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Core.Player;
using TMPro;
using UnityEngine;

namespace intheclouds
{
    public class PlayerMovementAP : MonoBehaviour
    {
        private HVRPlayerInputs playerInputs;
        private HVRPlayerController playerController;
        private Vector3 previousPosition;
        private PlayerStats playerStats;
        private float distanceMoved;

        //Debugging
        public TextMeshProUGUI distanceMovedText;

        private void OnEnable()
        {
            playerStats = GetComponent<PlayerStats>();
            var localUserObjects = transform.root.GetComponent<LocalUserObjects>();
            playerController = localUserObjects.HVRPlayerController;
            playerInputs = localUserObjects.HVRPlayerInputs;
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
            if (!playerStats.turn) return;
            if (playerInputs.LeftController.JoystickAxis.magnitude > 0.05f)
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