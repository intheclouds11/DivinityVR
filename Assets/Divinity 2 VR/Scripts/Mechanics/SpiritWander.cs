using System;
using System.Collections.Generic;
using HurricaneVR.Framework.ControllerInput;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class SpiritWander : MonoBehaviour
    {
        public GameObject[] objectsToDeparent;
        private Transform[] originalParents;
        private Vector3[] originalLocalPositions;
        private Quaternion[] originalLocalRotations;
        public GameObject tempParent;
        private AudioSource audioSource;
        public float timeInTriggerRequired = 1;
        public float grabTimeRequired = 1;
        public bool inTrigger;
        public float hoverTimeInTrigger;
        public float grabTimeInTrigger;
        public bool activated;
        public bool gripLatch;

        private void Start()
        {
            SaveParentsandTransforms();
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            ToggleAbilityCheck();
        }

        private void SaveParentsandTransforms()
        {
            if (originalParents == null) originalParents = new Transform[objectsToDeparent.Length];
            if (originalLocalPositions == null) originalLocalPositions = new Vector3[objectsToDeparent.Length];
            if (originalLocalRotations == null) originalLocalRotations = new Quaternion[objectsToDeparent.Length];

            for (int i = 0; i < objectsToDeparent.Length; i++)
            {
                originalParents[i] = objectsToDeparent[i].transform.parent;
                originalLocalPositions[i] = objectsToDeparent[i].transform.localPosition;
                originalLocalRotations[i] = objectsToDeparent[i].transform.localRotation;
                Debug.Log($"original localPos: {originalLocalPositions[i]}");
                Debug.Log($"original localRot: {originalLocalRotations[i]}");
            }
        }

        private void ToggleAbilityCheck()
        {
            if (!HVRInputManager.Instance.RightController.GripButtonState.Active)
            {
                gripLatch = false;
            }

            if (inTrigger)
            {
                if (hoverTimeInTrigger < 2)
                {
                    hoverTimeInTrigger += Time.time;
                }

                if (hoverTimeInTrigger >= timeInTriggerRequired)
                {
                    if (HVRInputManager.Instance.RightController.GripButtonState.Active)
                    {
                        if (grabTimeInTrigger < 2)
                        {
                            grabTimeInTrigger += Time.time;
                        }

                        if (!gripLatch && grabTimeInTrigger >= grabTimeRequired)
                        {
                            gripLatch = true;
                            ToggleSpiritForm();
                        }
                    }
                }
            }

            else if (!inTrigger)
            {
                if (hoverTimeInTrigger > 0)
                {
                    hoverTimeInTrigger -= Time.time;
                }

                if (grabTimeInTrigger > 0)
                {
                    grabTimeInTrigger -= Time.time;
                }
            }
        }

        private void ToggleSpiritForm()
        {
            audioSource.Play();
            if (!activated)
            {
                Deparent();
            }
            else
            {
                Reparent();
            }

            activated = !activated;
        }

        private void Deparent()
        {
            SaveParentsandTransforms();
            foreach (var o in objectsToDeparent)
            {
                o.transform.parent = tempParent.transform;
                if (o.layer == LayerMask.NameToLayer("InvisibleToMainCamera"))
                {
                    o.layer = LayerMask.NameToLayer("Default");
                    foreach (Transform child in o.transform)
                    {
                        child.gameObject.layer = LayerMask.NameToLayer("Default");
                    }
                }
            }
        }

        private void Reparent()
        {
            for (int i = 0; i < objectsToDeparent.Length; i++)
            {
                objectsToDeparent[i].transform.parent = originalParents[i];
                Debug.Log($" orig localPos: {originalLocalPositions[i]}");
                objectsToDeparent[i].transform.localPosition = originalLocalPositions[i];
                objectsToDeparent[i].transform.localRotation = originalLocalRotations[i];
                if (objectsToDeparent[i].layer == LayerMask.NameToLayer("Default"))
                {
                    objectsToDeparent[i].layer = LayerMask.NameToLayer("InvisibleToMainCamera");
                    foreach (Transform child in objectsToDeparent[i].transform)
                    {
                        child.gameObject.layer = LayerMask.NameToLayer("InvisibleToMainCamera");
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Hand"))
            {
                inTrigger = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Hand"))
            {
                inTrigger = false;
            }
        }
    }
}