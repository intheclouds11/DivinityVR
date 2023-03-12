using System;
using System.Collections.Generic;
using HurricaneVR.Framework.ControllerInput;
using UnityEngine;

namespace intheclouds
{
    public class SpiritWander : MonoBehaviour
    {
        public bool isActivated;
        public SpiritMovement spiritMovement;
        public Transform repositionTransformInitial;
        public GameObject[] objectsToDeparent;
        public Transform spawnParent;
        public float timeInTriggerRequired = 1;
        public bool inTriggerLH;
        public bool inTriggerRH;
        public float timeInTriggerLH;
        public float timeInTriggerRH;
        public List<GameObject> spawnedGOs;
        private Transform[] originalParents;
        private Vector3[] originalLocalPositions;
        private Quaternion[] originalLocalRotations;
        private Vector3 savedPhysicalPosition;
        private Quaternion savedPhysicalRotation;
        private Vector3 savedSpiritPosition;
        private Quaternion savedSpiritRotation;
        private LocalUserObjects playerLUOs;
        private AudioSource audioSource;

        public event Action SpiritFormToggled;

        private void Start()
        {
            playerLUOs = transform.GetComponentInParent<LocalUserObjects>();
            SaveOriginalTransforms();
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            InputCheck();
        }

        private void InputCheck()
        {
            if (inTriggerRH)
            {
                if (timeInTriggerRH < 2)
                {
                    timeInTriggerRH += Time.deltaTime;
                }

                if (timeInTriggerRH >= timeInTriggerRequired)
                {
                    if (HVRInputManager.Instance.RightController.TriggerButtonState.JustActivated)
                    {
                        ToggleSpiritForm();
                    }
                }
            }

            else if (!inTriggerRH)
            {
                if (timeInTriggerRH > 0)
                {
                    timeInTriggerRH = 0;
                }
            }
        }

        public void ToggleSpiritForm()
        {
            if (!playerLUOs.PlayerStats.InCombat)
            {
                return;
            }

            audioSource.Play();
            if (!isActivated)
            {
                Separate();
                Reposition();
                spiritMovement.enabled = true;
            }
            else
            {
                Reunite();
                Reposition();
                spiritMovement.enabled = false;
            }

            isActivated = !isActivated;
            SpiritFormToggled?.Invoke();
        }

        private void Reposition()
        {
            if (!isActivated)
            {
                if (savedSpiritPosition != Vector3.zero && savedSpiritRotation != Quaternion.identity)
                {
                    playerLUOs.HVRPlayerController.transform.position = savedSpiritPosition;
                    playerLUOs.HVRPlayerController.transform.rotation = savedSpiritRotation;
                }
                else
                {
                    playerLUOs.HVRPlayerController.transform.position = repositionTransformInitial.position;
                }
            }

            else
            {
                playerLUOs.HVRPlayerController.transform.position = savedPhysicalPosition;
                playerLUOs.HVRPlayerController.transform.rotation = savedPhysicalRotation;
            }
        }

        // save original transform, spawn another instance of the player and remove any unnecessary components
        private void Separate()
        {
            SaveOriginalTransforms();
            spawnedGOs = new List<GameObject>();
            foreach (var obj in objectsToDeparent)
            {
                GameObject physicalFormObject = Instantiate(obj, obj.transform.position, obj.transform.rotation, spawnParent);
                spawnedGOs.Add(physicalFormObject);
                var components = physicalFormObject.GetComponents<Component>();
                var childComponents = physicalFormObject.GetComponentsInChildren<Component>();
                if (physicalFormObject.CompareTag("Headwear"))
                {
                    physicalFormObject.layer = LayerMask.NameToLayer("Default");
                }

                foreach (var component in components)
                {
                    if (component is not (Transform or SkinnedMeshRenderer or MeshRenderer or MeshFilter))
                    {
                        Destroy(component);
                    }
                }

                foreach (var childComponent in childComponents)
                {
                    if (childComponent is not (Transform or SkinnedMeshRenderer or MeshRenderer or MeshFilter))
                    {
                        Destroy(childComponent);
                    }
                }
            }

            // spawn new visor (without socket)
            var visorSocketGO = transform.GetComponentInParent<LocalUserObjects>().visorSocket;
            if (visorSocketGO.transform.childCount > 0) // if socket not empty
            {
                GameObject visorOriginal = visorSocketGO.transform.GetChild(0).gameObject;
                GameObject visorSpawned = Instantiate(visorOriginal, visorOriginal.transform.position, visorOriginal.transform.rotation, spawnParent);
                spawnedGOs.Add(visorSpawned);
            }
        }

        // Return to position and destroy spawnedObjs
        private void Reunite()
        {
            savedSpiritPosition = playerLUOs.HVRPlayerController.transform.position;
            savedSpiritRotation = playerLUOs.HVRPlayerController.transform.rotation;

            foreach (var spawnedGO in spawnedGOs)
            {
                Destroy(spawnedGO);
            }

            spawnedGOs = null;
        }

        private void SaveOriginalTransforms()
        {
            if (originalLocalPositions == null) originalLocalPositions = new Vector3[objectsToDeparent.Length];
            if (originalLocalRotations == null) originalLocalRotations = new Quaternion[objectsToDeparent.Length];
            savedPhysicalPosition = playerLUOs.HVRPlayerController.transform.position;
            savedPhysicalRotation = playerLUOs.HVRPlayerController.transform.rotation;

            for (int i = 0; i < objectsToDeparent.Length; i++)
            {
                originalLocalPositions[i] = objectsToDeparent[i].transform.localPosition;
                originalLocalRotations[i] = objectsToDeparent[i].transform.localRotation;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Right Hand"))
            {
                inTriggerRH = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Right Hand"))
            {
                inTriggerRH = false;
            }
        }
    }
}