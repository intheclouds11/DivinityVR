using HurricaneVR.Framework.ControllerInput;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Utils;
using HurricaneVR.Framework.Shared;
using UnityEngine;

namespace intheclouds
{
    public class CreditPickup : MonoBehaviour, IHoverableItem
    {
        public int credits;
        private HVRGrabbable _grabbable;

        private void Start()
        {
            _grabbable = GetComponent<HVRGrabbable>();
        }

        private void Update()
        {
            if (_grabbable.HandGrabbers.Count > 0)
            {
                var hand = _grabbable.HandGrabbers[0];
                var value = hand.Controller.ControllerType == HVRControllerType.Knuckles ? hand.Controller.GripForce : hand.Controller.Trigger;

                if (value > 0.5f)
                {
                    LocalUserObjects.instance.PlayerStats.UpdateCredits(credits);
                    SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.creditPickupSFX, transform.position);
                    hand.Controller.Vibrate(HVRInputManager.Instance.HandInputHaptics.ForceGrab);
                    Destroy(gameObject);
                }
            }
        }

        public string GetHoverInfo()
        {
            return $"Credits: {credits}";
        }
    }
}
