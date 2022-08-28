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
        public Camera Camera;
        public GameObject leftController;
        public GameObject rightController;
        // todo: set the Rigidbody on these to isKinematic when !activePlayer
        public GameObject leftHandPhysics;
        public GameObject rightHandPhysics;
        public bool activePlayer;
    }
}
