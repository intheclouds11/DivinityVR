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

        private void Awake()
        {
            playerInputs = GetComponent<HVRPlayerInputs>();
            playerController = GetComponent<HVRPlayerController>();
            playerStats = GetComponent<PlayerStats>();
        }

        private void Update()
        {
            if (!playerStats.turn)
            {
                if (!playerStats.explorationMode) playerController.MovementEnabled = false;
                return;
            }

            if (playerInputs.LeftController.JoystickAxis.magnitude > 0.05f)
            {
                TrackMovementApUsage();
                distanceMovedText.text = $"distance moved: {(int) distanceMoved}";
            }
        }

        // todo: call this when Combat Game Mode starts
        public void StartTurnSetup()
        {
            previousPosition = transform.position;
            distanceMoved = 0;
        }

        private void TrackMovementApUsage()
        {
            distanceMoved += Vector3.Distance(transform.position, previousPosition);

            if (distanceMoved > 3)
            {
                playerStats.UseAP(1);
                distanceMoved -= 3;
            }

            previousPosition = transform.position;
        }
    }
}