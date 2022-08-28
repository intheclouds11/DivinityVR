using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace intheclouds
{
    public class PlayerSwapper : MonoBehaviour
    {
        private PlayerStats currentControlledPlayer;
        
        public IEnumerator SwapPlayer()
        {
            foreach (var player in GameManager.Instance.players)
            {
                // First get currently controlled player
                while (currentControlledPlayer == null)
                {
                    if (player.playerControlled)
                    {
                        currentControlledPlayer = player;
                        yield return null;
                    }
                }

                // Then if that player is the same as button text, exit coroutine
                if (currentControlledPlayer.Name == GetComponentInChildren<TextMeshProUGUI>().text)
                {
                    Debug.Log("Character is already being controlled");
                    StopCoroutine(SwapPlayer());
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
            var swappedPlayerObjects = currentControlledPlayer.GetComponentInParent<LocalUserObjects>();
            swappedPlayerObjects.activePlayer = true;
            swappedPlayerObjects.HVRPlayerController.enabled = true;
            swappedPlayerObjects.HVRPlayerInputs.enabled = true;
            swappedPlayerObjects.ITCPlayerInputs.enabled = true;
            swappedPlayerObjects.PlayerMovementAP.enabled = true;
            swappedPlayerObjects.leftController.SetActive(true);
            swappedPlayerObjects.rightController.SetActive(true);
            swappedPlayerObjects.leftHandPhysics.GetComponent<Rigidbody>().isKinematic = false;
            swappedPlayerObjects.rightHandPhysics.GetComponent<Rigidbody>().isKinematic = false;
            var cameraGOComponents = swappedPlayerObjects.Camera.gameObject.GetComponents<MonoBehaviour>();
            foreach (var component in cameraGOComponents)
            {
                component.enabled = true;
            }
        }

        private void DisableCurrentPlayerObjects()
        {
            var currentPlayerObjects = currentControlledPlayer.GetComponentInParent<LocalUserObjects>();
            currentPlayerObjects.activePlayer = false;
            currentPlayerObjects.HVRPlayerController.enabled = false;
            currentPlayerObjects.HVRPlayerInputs.enabled = false;
            currentPlayerObjects.ITCPlayerInputs.enabled = false;
            currentPlayerObjects.PlayerMovementAP.enabled = false;
            currentPlayerObjects.leftController.SetActive(false);
            currentPlayerObjects.rightController.SetActive(false);
            currentPlayerObjects.leftHandPhysics.GetComponent<Rigidbody>().isKinematic = true;
            currentPlayerObjects.rightHandPhysics.GetComponent<Rigidbody>().isKinematic = true;
            var cameraComponents = currentPlayerObjects.Camera.gameObject.GetComponents<MonoBehaviour>();
            foreach (var cameraComponent in cameraComponents)
            {
                cameraComponent.enabled = false;
            }
        }
    }
}