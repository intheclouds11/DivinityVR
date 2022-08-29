using HurricaneVR.Framework.Components;
using HurricaneVR.Framework.Core.HandPoser;
using HurricaneVR.Framework.Core.Player;
using TMPro;
using UnityEngine;
using UnityEngine.SpatialTracking;

namespace intheclouds
{
    public class PlayerSwapper : MonoBehaviour
    {
        private PlayerStats currentControlledPlayer;

        public void PlayerSwap()
        {
            restart:
            foreach (var player in GameManager.Instance.players)
            {
                // First get currently controlled player
                if (currentControlledPlayer == null)
                {
                    if (player.playerControlled)
                    {
                        currentControlledPlayer = player;
                        goto restart;
                    }
                    else
                    {
                        continue;
                    }
                }

                // Then if that player is the same as button text, exit coroutine
                if (currentControlledPlayer.Name == GetComponentInChildren<TextMeshProUGUI>().text)
                {
                    Debug.Log("Character is already being controlled");
                    return;
                }

                if (player.Name == GetComponentInChildren<TextMeshProUGUI>().text)
                {
                    DisableCurrentPlayerObjects();
                    EnableSwappedPlayerObjects(player);
                }
            }
        }

        private void EnableSwappedPlayerObjects(PlayerStats playerStats)
        {
            var swappedPlayerObjects = playerStats.GetComponentInParent<LocalUserObjects>();
            swappedPlayerObjects.PlayerStats.playerControlled = true;
            swappedPlayerObjects.HVRPlayerController.enabled = true;
            swappedPlayerObjects.HVRPlayerInputs.enabled = true;
            swappedPlayerObjects.ITCPlayerInputs.enabled = true;

            swappedPlayerObjects.leftController.GetComponent<TrackedPoseDriver>().enabled = true;
            swappedPlayerObjects.leftHandModel.GetComponent<HVRHandAnimator>().enabled = true;
            swappedPlayerObjects.leftHandPhysics.GetComponent<Rigidbody>().isKinematic = false;
            swappedPlayerObjects.leftHandPhysics.GetComponent<HVRJointHand>().Target = swappedPlayerObjects.leftController.GetComponentInChildren<HVRControllerOffset>().transform;
            
            swappedPlayerObjects.rightController.GetComponent<TrackedPoseDriver>().enabled = true;
            swappedPlayerObjects.rightHandModel.GetComponent<HVRHandAnimator>().enabled = true;
            swappedPlayerObjects.rightHandPhysics.GetComponent<Rigidbody>().isKinematic = false;
            swappedPlayerObjects.rightHandPhysics.GetComponent<HVRJointHand>().Target = swappedPlayerObjects.rightController.GetComponentInChildren<HVRControllerOffset>().transform;

            var cameraGOComponents = swappedPlayerObjects.Camera.gameObject.GetComponents<Behaviour>();
            foreach (var component in cameraGOComponents)
            {
                component.enabled = true;
            }

            UserMenu.Instance.UserSetup(swappedPlayerObjects.PlayerStats);
        }

        private void DisableCurrentPlayerObjects()
        {
            var currentPlayerObjects = currentControlledPlayer.GetComponentInParent<LocalUserObjects>();
            currentPlayerObjects.PlayerStats.playerControlled = false;
            currentPlayerObjects.HVRPlayerController.enabled = false;
            currentPlayerObjects.HVRPlayerInputs.enabled = false;
            currentPlayerObjects.ITCPlayerInputs.enabled = false;

            currentPlayerObjects.leftController.GetComponent<TrackedPoseDriver>().enabled = false;
            currentPlayerObjects.leftHandModel.GetComponent<HVRHandAnimator>().enabled = false;
            currentPlayerObjects.leftHandPhysics.GetComponent<Rigidbody>().isKinematic = true;
            currentPlayerObjects.leftHandPhysics.GetComponent<HVRJointHand>().Target = currentPlayerObjects.leftHandPhysics.transform;
            
            currentPlayerObjects.rightController.GetComponent<TrackedPoseDriver>().enabled = false;
            currentPlayerObjects.rightHandModel.GetComponent<HVRHandAnimator>().enabled = false;
            currentPlayerObjects.rightHandPhysics.GetComponent<Rigidbody>().isKinematic = true;
            currentPlayerObjects.rightHandPhysics.GetComponent<HVRJointHand>().Target = currentPlayerObjects.rightHandPhysics.transform;

            var cameraComponents = currentPlayerObjects.Camera.gameObject.GetComponents<Behaviour>();
            foreach (var cameraComponent in cameraComponents)
            {
                cameraComponent.enabled = false;
            }
        }
    }
}