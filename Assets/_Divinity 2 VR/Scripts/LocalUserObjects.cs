using System;
using HighlightPlus;
using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Core.Player;
using UnityEngine;

namespace intheclouds
{
    // NOTE: do not child Player to anything.
    public class LocalUserObjects : MonoBehaviour
    {
        public PlayerStats PlayerStats;
        public HVRPlayerController HVRPlayerController;
        public HVRPlayerInputs HVRPlayerInputs;
        public ITCPlayerInputs ITCPlayerInputs;
        public PlayerMovementAP PlayerMovementAP;
        public HVRCameraRig HVRCameraRig;
        public Camera Camera;
        public GameObject leftController;
        public GameObject leftHandPhysics;
        public GameObject leftHandModel;
        public GameObject leftHandPalm;
        public GameObject rightController;
        public GameObject rightHandPhysics;
        public GameObject rightHandModel;
        public GameObject rightHandPalm;
        public GameObject userMenuSpawnPoint;
        public GameObject visorSocket;
        public GameObject turnOrderUI;
        public SpiritWander spiritWander;
        public HighlightEffect handAugmentHighlight;
        public MagicSelector magicSelector;
        public GameObject skills;
        
        private void Start()
        {
            if (PlayerStats.PlayerControlled)
            {
                turnOrderUI.transform.SetParent(Camera.transform, false);
                turnOrderUI.transform.localPosition = Vector3.zero;
                turnOrderUI.transform.localRotation = Quaternion.identity;
            }
        }

        private void Update()
        {
            if (magicSelector.selectedMagic && magicSelector.selectedMagic.activeInHierarchy)
            {
                magicSelector.selectedMagic.transform.parent = leftHandPalm.transform;
                if (magicSelector.selectedMagic.transform.localPosition != Vector3.zero)
                {
                    magicSelector.selectedMagic.transform.localPosition = Vector3.zero;
                    magicSelector.selectedMagic.transform.localRotation = Quaternion.identity;
                }
            }
        }
    }
    
    
}
