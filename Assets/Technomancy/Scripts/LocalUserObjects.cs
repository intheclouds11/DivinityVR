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
        public GameObject leftHandAbilitySelectorSpawn;
        public GameObject rightController;
        public GameObject rightHandPhysics;
        public GameObject rightHandModel;
        public GameObject rightHandPalm;
        public GameObject rightHandAbilitySelectorSpawn;
        public GameObject waist;
        public GameObject userMenu;
        public GameObject userMenuSpawnPoint;
        public GameObject visorSocket;
        public GameObject turnOrderUI;
        public SpiritWander spiritWander;
        public HighlightEffect handAugmentHighlight;
        public AbilitySystem abilitySystem;
        public GameObject abilities;
        public AbilityPointer leftAbilityPointer;
        public AbilityPointer rightAbilityPointer;

        private void Start()
        {
            if (PlayerStats.PlayerControlled)
            {
                turnOrderUI.transform.SetParent(Camera.transform, false);
                turnOrderUI.transform.localPosition = Vector3.zero;
                turnOrderUI.transform.localRotation = Quaternion.identity;
            }
        }
    }
    
    
}
