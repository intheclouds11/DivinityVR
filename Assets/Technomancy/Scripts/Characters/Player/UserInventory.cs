using System;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.Utils;
using HurricaneVR.Framework.Shared;
using UnityEngine;

namespace intheclouds
{
    public class UserInventory : MonoBehaviour
    {
        public HVRHandGrabber LeftHandGrabber;
        public HVRHandGrabber RightHandGrabber;
        public TriggerEvents LeftHandLockTrigger;
        public TriggerEvents RightHandLockTrigger;

        private void Awake()
        {
            LeftHandLockTrigger.ExceededTimeInTriggerAction += LeftHandActions;
            RightHandLockTrigger.ExceededTimeInTriggerAction += RightHandActions;
        }

        private void LeftHandActions()
        {
            if (LeftHandGrabber.GrabTrigger != HVRGrabTrigger.ManualRelease && LeftHandGrabber.GrabbedTarget)
            {
                Debug.Log($"Equipped: {LeftHandGrabber.GrabbedTarget}");
                LeftHandGrabber.GrabTrigger = HVRGrabTrigger.ManualRelease;
                SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, LocalUserObjects.Instance.Camera.transform.position, 1.2f, 1);
            }
            else if (LeftHandGrabber.GrabTrigger == HVRGrabTrigger.ManualRelease)
            {
                Debug.Log($"Dequipped: {LeftHandGrabber.GrabbedTarget}");
                LeftHandGrabber.GrabTrigger = HVRGrabTrigger.Toggle;
                SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, LocalUserObjects.Instance.Camera.transform.position, 0.8f, 1);
            }
        }

        private void RightHandActions()
        {
            if (RightHandGrabber.GrabTrigger != HVRGrabTrigger.ManualRelease && RightHandGrabber.GrabbedTarget)
            {
                Debug.Log($"Equipped: {RightHandGrabber.GrabbedTarget}");
                RightHandGrabber.GrabTrigger = HVRGrabTrigger.ManualRelease;
                SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, LocalUserObjects.Instance.Camera.transform.position, 1.2f, 1);
            }
            else if (RightHandGrabber.GrabTrigger == HVRGrabTrigger.ManualRelease)
            {
                Debug.Log($"Dequipped: {RightHandGrabber.GrabbedTarget}");
                RightHandGrabber.GrabTrigger = HVRGrabTrigger.Toggle;
                SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, LocalUserObjects.Instance.Camera.transform.position, 0.8f, 1);
            }
        }
    }
}