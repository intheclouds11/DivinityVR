using System;
using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Core.Player;
using intheclouds;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class PlayerMovementAP : MonoBehaviour
    {
        public bool activated;
        private HVRPlayerInputs playerInputs;
        private HVRPlayerController playerController;
        private Vector3 previousPosition;
        private PlayerStats playerStats;
        private float distanceMoved;
        private int apConsumed;

        //Debugging
        public TextMeshProUGUI distanceMovedText;
        public TextMeshProUGUI currentAPText;

        private void Start()
        {
            playerInputs = GetComponent<HVRPlayerInputs>();
            playerController = GetComponent<HVRPlayerController>();
        }

        private void Update()
        {
            if (!activated)
            {
                return;
            }

            if (playerStats.currentAP == 0)
            {
                playerController.MovementEnabled = false;
                activated = false;
                Debug.Log("Out of AP. Movement disabled.");
                return;
            }

            if (playerInputs.LeftController.JoystickAxis.magnitude > 0.05f)
            {
                UseAP();
                distanceMovedText.text = $"distance moved: {(int) distanceMoved}";
                currentAPText.text = $"AP consumed: {playerStats.currentAP}";
            }
        }

        public void StartTurnSetup()
        {
            activated = true;
            previousPosition = transform.position;
            distanceMoved = 0;
            apConsumed = 0;

            APManager.instance.playersStatsDictionary.TryGetValue(this.gameObject, out var stats);
            playerStats = stats;
            if (playerStats == null)
            {
                Debug.LogError("Couldn't get PlayerStats from APManager");
                activated = false;
                return;
            }
        }

        private void UseAP()
        {
            distanceMoved += Vector3.Distance(transform.position, previousPosition);

            if (distanceMoved > 3)
            {
                playerStats.currentAP -= 1;
                distanceMoved -= 3;
            }

            previousPosition = transform.position;
        }
    }
}