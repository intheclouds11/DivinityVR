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
        private Transform[] _originalParents;
        private Vector3[] _originalLocalPositions;
        private Quaternion[] _originalLocalRotations;
        private Vector3 _savedPhysicalPosition;
        private Quaternion _savedPhysicalRotation;
        private Vector3 _savedSpiritPosition;
        private Quaternion _savedSpiritRotation;
        private LocalUserObjects _playerLuOs;
        private AudioSource _audioSource;

        public event Action SpiritFormToggled;

        private void Start()
        {
            _playerLuOs = transform.GetComponentInParent<LocalUserObjects>();
            SaveOriginalTransforms();
            _audioSource = GetComponent<AudioSource>();
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
            if (!_playerLuOs.PlayerStats.InCombat && !Startup.instance.debugMode)
            {
                return;
            }

            _audioSource.Play();
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
                if (_savedSpiritPosition != Vector3.zero && _savedSpiritRotation != Quaternion.identity)
                {
                    _playerLuOs.ITCPlayerController.transform.position = _savedSpiritPosition;
                    _playerLuOs.ITCPlayerController.transform.rotation = _savedSpiritRotation;
                }
                else
                {
                    _playerLuOs.ITCPlayerController.transform.position = repositionTransformInitial.position;
                }
            }

            else
            {
                _playerLuOs.ITCPlayerController.transform.position = _savedPhysicalPosition;
                _playerLuOs.ITCPlayerController.transform.rotation = _savedPhysicalRotation;
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
            _savedSpiritPosition = _playerLuOs.ITCPlayerController.transform.position;
            _savedSpiritRotation = _playerLuOs.ITCPlayerController.transform.rotation;

            foreach (var spawnedGO in spawnedGOs)
            {
                Destroy(spawnedGO);
            }

            spawnedGOs = null;
        }

        private void SaveOriginalTransforms()
        {
            if (_originalLocalPositions == null) _originalLocalPositions = new Vector3[objectsToDeparent.Length];
            if (_originalLocalRotations == null) _originalLocalRotations = new Quaternion[objectsToDeparent.Length];
            _savedPhysicalPosition = _playerLuOs.ITCPlayerController.transform.position;
            _savedPhysicalRotation = _playerLuOs.ITCPlayerController.transform.rotation;

            for (int i = 0; i < objectsToDeparent.Length; i++)
            {
                _originalLocalPositions[i] = objectsToDeparent[i].transform.localPosition;
                _originalLocalRotations[i] = objectsToDeparent[i].transform.localRotation;
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