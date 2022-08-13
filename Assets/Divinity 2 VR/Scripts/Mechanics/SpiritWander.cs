using System.Collections.Generic;
using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Core.Player;
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
            hvrPlayerController = transform.parent.parent.GetComponentInChildren<HVRPlayerController>();
            SaveOriginalTransforms();
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            InputCheck();
        }

        private void InputCheck()
        {
            // if (inTriggerLH)
            // {
            //     if (timeInTriggerLH < 2)
            //     {
            //         timeInTriggerLH += Time.deltaTime;
            //     }
            //
            //     if (timeInTriggerLH >= timeInTriggerRequired)
            //     {
            //         if (HVRInputManager.Instance.LeftController.GripButtonState.JustActivated)
            //         {
            //             ToggleSpiritForm();
            //         }
            //     }
            // }
            //
            // else if (!inTriggerLH)
            // {
            //     timeInTriggerLH -= Time.deltaTime;
            // }

            if (inTriggerRH)
            {
                if (timeInTriggerRH < 2)
                {
                    timeInTriggerRH += Time.deltaTime;
                }

                if (timeInTriggerRH >= timeInTriggerRequired)
                {
                    if (HVRInputManager.Instance.RightController.GripButtonState.JustActivated)
                    {
                        ToggleSpiritForm();
                    }
                }
            }

            else if (!inTriggerRH)
            {
                if (timeInTriggerRH > 0)
                {
                    timeInTriggerRH -= Time.deltaTime;
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
                // var physicalFormObject = Instantiate(obj, obj.transform.position, obj.transform.rotation);
                var physicalFormObject = Instantiate(obj, obj.transform.position, obj.transform.rotation, spawnParent);
                spawnedGOs.Add(physicalFormObject);
                var components = physicalFormObject.GetComponents<Component>();
                var childComponents = physicalFormObject.GetComponentsInChildren<Component>();
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

                // make any masked objects visible to VRcamera
                physicalFormObject.layer = LayerMask.NameToLayer("Default");
                foreach (Transform child in physicalFormObject.transform)
                {
                    child.gameObject.layer = LayerMask.NameToLayer("Default");
                }
            }
        }

        // Return to position and destroy spawnedObjs
        private void Reunite()
        {
            hvrPlayerController.transform.position = initialCharacterPosition;
            hvrPlayerController.transform.rotation = initialCharacterRotation;

            foreach (var o in spawnedGOs)
            {
                Destroy(o);
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