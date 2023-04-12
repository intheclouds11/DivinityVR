using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Utils;
using HurricaneVR.Framework.Shared;
using UnityEngine;

namespace intheclouds
{
    public class CreditPickup : MonoBehaviour
    {
        public int credits;
        private HVRGrabbable grabbable;

        private void Start()
        {
            grabbable = GetComponent<HVRGrabbable>();
        }

        private void Update()
        {
            if (grabbable.HandGrabbers.Count > 0)
            {
                var hand = grabbable.HandGrabbers[0];
                var value = hand.Controller.ControllerType == HVRControllerType.Knuckles ? hand.Controller.GripForce : hand.Controller.Trigger;

                if (value > 0.5f)
                {
                    LocalUserObjects.Instance.PlayerStats.Gold += credits;
                    SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.creditPickupSFX, transform.position);
                    Destroy(gameObject);
                }
            }
        }
    }
}
