using HighlightPlus;
using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.Player;
using UnityEngine;

namespace intheclouds
{
    public class LocalUserObjects : MonoBehaviour
    {
        public static LocalUserObjects instance;
        public PlayerStats PlayerStats;
        public ITCPlayerController ITCPlayerController;
        public ITCTeleporter ITCTeleporter;
        public HVRPlayerInputs HVRPlayerInputs;
        public ITCPlayerInputs ITCPlayerInputs;
        public PlayerMovementAP PlayerMovementAP;
        public HVRCameraRig HVRCameraRig;
        public Camera Camera;
        public PlayerHUDController HUDController;
        public Transform leftController;
        public HVRHandGrabber leftHandGrabber;
        public GameObject leftHandModel;
        public GameObject leftHandPalm;
        public GameObject leftHandAbilitySelectorSpawn;
        public Transform rightController;
        public HVRHandGrabber rightHandGrabber;
        public GameObject rightHandModel;
        public GameObject rightHandPalm;
        public GameObject rightHandAbilitySelectorSpawn;
        public GameObject waist;
        public GameObject userMenuSpawnPoint;
        public GameObject visorSocket;
        public GameObject turnOrderUI;
        public SpiritWander spiritWander;
        public HighlightEffect handAugmentHighlight;
        public AbilitySystem abilitySystem;
        public GameObject abilities;
        public AbilityPointer leftAbilityPointer;
        public AbilityPointer rightAbilityPointer;

        private void Awake()
        {
            instance = this;
        }
    }
}
