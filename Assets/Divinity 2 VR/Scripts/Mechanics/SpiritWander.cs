using System.Collections.Generic;
using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.Player;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;

namespace intheclouds
{
    public class SpiritWander : MonoBehaviour
    {
        public bool activated;
        public SpiritMovement spiritMovement;
        public Transform repositionTransform;
        public GameObject[] objectsToDeparent;
        public Transform spawnParent;
        public float timeInTriggerRequired = 1;
        public bool inTriggerLH;
        public bool inTriggerRH;
        public float timeInTriggerLH;
        public float timeInTriggerRH;
        private List<GameObject> spawnedGOs;
        private Transform[] originalParents;
        private Vector3[] originalLocalPositions;
        private Quaternion[] originalLocalRotations;
        private Vector3 initialCharacterPosition;
        private Quaternion initialCharacterRotation;
        private HVRPlayerController hvrPlayerController;
        private AudioSource audioSource;

        private void Start()
        {
            hvrPlayerController = transform.root.GetComponent<LocalUserObjects>().HVRPlayerController;
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

        // toggle spirit form and SpiritMovement.cs
        public void ToggleSpiritForm()
        {
            audioSource.Play();
            if (!activated)
            {
                Separate();
                hvrPlayerController.transform.position = repositionTransform.position;
                spiritMovement.enabled = true;
            }
            else
            {
                Reunite();
                spiritMovement.enabled = false;
            }

            activated = !activated;
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
            var visorSocketGO = transform.root.GetComponent<LocalUserObjects>().visorSocket;
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
            hvrPlayerController.transform.position = initialCharacterPosition;
            hvrPlayerController.transform.rotation = initialCharacterRotation;

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
            initialCharacterPosition = hvrPlayerController.transform.position;
            initialCharacterRotation = hvrPlayerController.transform.rotation;

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