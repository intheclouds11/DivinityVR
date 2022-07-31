using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Core.Player;
using UnityEngine;

namespace intheclouds
{
    public class SpiritWander : MonoBehaviour
    {
        public GameObject[] objectsToDeparent;
        private Transform[] originalParents;
        private Vector3[] originalLocalPositions;
        private Quaternion[] originalLocalRotations;
        private Vector3 originalCharacterPosition;
        private Quaternion originalCharacterRotation;
        private HVRPlayerController hvrPlayerController;
        public GameObject tempParent;
        private AudioSource audioSource;
        public float timeInTriggerRequired = 1;
        public float grabTimeRequired = 1;
        public bool inTriggerLH;
        public bool inTriggerRH;
        public float hoverTimeInTriggerLH;
        public float grabTimeInTriggerLH;
        public float hoverTimeInTriggerRH;
        public float grabTimeInTriggerRH;
        public bool activated;
        public bool gripLatchLH;
        public bool gripLatchRH;

        public SpiritMovement spiritMovement;

        private void Start()
        {
            hvrPlayerController = transform.parent.parent.GetComponentInChildren<HVRPlayerController>();
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
            originalCharacterPosition = hvrPlayerController.transform.position;
            originalCharacterRotation = hvrPlayerController.transform.rotation;

            for (int i = 0; i < objectsToDeparent.Length; i++)
            {
                originalParents[i] = objectsToDeparent[i].transform.parent;
                originalLocalPositions[i] = objectsToDeparent[i].transform.localPosition;
                originalLocalRotations[i] = objectsToDeparent[i].transform.localRotation;
            }
        }

        private void ToggleAbilityCheck()
        {
            if (!HVRInputManager.Instance.RightController.GripButtonState.Active)
            {
                gripLatchRH = false;
            }

            if (!HVRInputManager.Instance.LeftController.GripButtonState.Active)
            {
                gripLatchLH = false;
            }

            if (inTriggerLH)
            {
                if (hoverTimeInTriggerLH < 2)
                {
                    hoverTimeInTriggerLH += Time.time;
                }

                if (hoverTimeInTriggerLH >= timeInTriggerRequired)
                {
                    if (HVRInputManager.Instance.LeftController.GripButtonState.Active)
                    {
                        if (grabTimeInTriggerLH < 2)
                        {
                            grabTimeInTriggerLH += Time.time;
                        }

                        if (!gripLatchLH && grabTimeInTriggerLH >= grabTimeRequired)
                        {
                            if (grabTimeInTriggerRH > grabTimeInTriggerLH) return;
                            gripLatchLH = true;
                            ToggleSpiritForm();
                        }
                    }
                }
            }

            else if (!inTriggerLH)
            {
                if (hoverTimeInTriggerLH > 0)
                {
                    hoverTimeInTriggerLH -= Time.time;
                }

                if (grabTimeInTriggerLH > 0)
                {
                    grabTimeInTriggerLH -= Time.time;
                }
            }

            if (inTriggerRH)
            {
                if (hoverTimeInTriggerRH < 2)
                {
                    hoverTimeInTriggerRH += Time.time;
                }

                if (hoverTimeInTriggerRH >= timeInTriggerRequired)
                {
                    if (HVRInputManager.Instance.RightController.GripButtonState.Active)
                    {
                        if (grabTimeInTriggerRH < 2)
                        {
                            grabTimeInTriggerRH += Time.time;
                        }

                        if (!gripLatchRH && grabTimeInTriggerRH >= grabTimeRequired)
                        {
                            if (grabTimeInTriggerLH > grabTimeInTriggerRH) return;
                            gripLatchRH = true;
                            ToggleSpiritForm();
                        }
                    }
                }
            }

            else if (!inTriggerRH)
            {
                if (hoverTimeInTriggerRH > 0)
                {
                    hoverTimeInTriggerRH -= Time.time;
                }

                if (grabTimeInTriggerRH > 0)
                {
                    grabTimeInTriggerRH -= Time.time;
                }
            }
        }

        private void ToggleSpiritForm()
        {
            audioSource.Play();
            if (!activated)
            {
                Deparent();
                spiritMovement.enabled = true;
            }
            else
            {
                Reparent();
                spiritMovement.enabled = false;
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
            hvrPlayerController.transform.position = originalCharacterPosition;
            hvrPlayerController.transform.rotation = originalCharacterRotation;
            for (int i = 0; i < objectsToDeparent.Length; i++)
            {
                objectsToDeparent[i].transform.parent = originalParents[i];
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
            if (other.CompareTag("Left Hand"))
            {
                inTriggerLH = true;
            }

            if (other.CompareTag("Right Hand"))
            {
                inTriggerRH = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Left Hand"))
            {
                inTriggerLH = false;
            }

            if (other.CompareTag("Right Hand"))
            {
                inTriggerRH = false;
            }
        }
    }
}