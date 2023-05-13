using System.Collections;
using System.Linq;
using HighlightPlus;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.Sockets;
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
        public TriggerEvents LeftHandSocketsTrigger;
        public TriggerEvents RightHandSocketsTrigger;
        private HVRSocket[] leftHandSockets;
        private HVRSocket[] rightHandSockets;
        private HVRSocketHoverScale[] leftHandSocketsActions;
        private HVRSocketHoverScale[] rightHandSocketsActions;
        private HighlightEffect leftHandLockTriggerHighlight;
        private HighlightEffect rightHandLockTriggerHighlight;
        
        private Coroutine hideLeftSocketsCoroutine;
        private Coroutine hideRightSocketsCoroutine;

        private void Awake()
        {
            leftHandSockets = LeftHandSocketsTrigger.GetComponentsInChildren<HVRSocket>(true);
            rightHandSockets = RightHandSocketsTrigger.GetComponentsInChildren<HVRSocket>(true);
            leftHandSocketsActions = LeftHandSocketsTrigger.GetComponentsInChildren<HVRSocketHoverScale>(true);
            rightHandSocketsActions = RightHandSocketsTrigger.GetComponentsInChildren<HVRSocketHoverScale>(true);
            leftHandLockTriggerHighlight = LeftHandLockTrigger.GetComponent<HighlightEffect>();
            rightHandLockTriggerHighlight = RightHandLockTrigger.GetComponent<HighlightEffect>();
            
            LeftHandSocketsTrigger.TriggerEnterEvent.AddListener(HandInventoryTriggerEnter);
            LeftHandSocketsTrigger.TriggerExitEvent.AddListener(HandInventoryTriggerExit);
            RightHandSocketsTrigger.TriggerEnterEvent.AddListener(HandInventoryTriggerEnter);
            RightHandSocketsTrigger.TriggerExitEvent.AddListener(HandInventoryTriggerExit);
            LeftHandLockTrigger.ExceededRequiredTime.AddListener(HandLockTriggerExceededTime);
            LeftHandLockTrigger.TriggerEnterEvent.AddListener(HandLockTriggerEnter);
            LeftHandLockTrigger.TriggerExitEvent.AddListener(HandLockTriggerExit);
            RightHandLockTrigger.ExceededRequiredTime.AddListener(HandLockTriggerExceededTime);
            RightHandLockTrigger.TriggerEnterEvent.AddListener(HandLockTriggerEnter);
            RightHandLockTrigger.TriggerExitEvent.AddListener(HandLockTriggerExit);
        }

        private void HandInventoryTriggerEnter(HVRHandSide handSide)
        {
            if (handSide == HVRHandSide.Left && !RightHandGrabber.HoverTarget && !RightHandGrabber.ForceGrabber.HoverTarget)
            {
                foreach (var leftHandSocket in leftHandSockets)
                {
                    leftHandSocket.gameObject.SetActive(true);
                }
            }
            else if (handSide == HVRHandSide.Right && !LeftHandGrabber.HoverTarget && !LeftHandGrabber.ForceGrabber.HoverTarget)
            {
                foreach (var rightHandSocket in rightHandSockets)
                {
                    rightHandSocket.gameObject.SetActive(true);
                }
            }
        }
        
        private void HandInventoryTriggerExit(HVRHandSide handSide)
        {
            if (handSide == HVRHandSide.Left && hideLeftSocketsCoroutine == null)
            {
                bool leftHandHoverRoutinePlaying = false;
                foreach (var leftHandSocket in leftHandSockets)
                {
                    foreach (var hvrSocketHoverAction in leftHandSocket.HandGrabActions)
                    { 
                        if (hvrSocketHoverAction._hoverRoutine != null)
                        {
                            leftHandHoverRoutinePlaying = true;
                            break;
                        }
                    }

                    if (leftHandSocket.IsHovering || leftHandSocket.HandHovering || leftHandHoverRoutinePlaying)
                    {
                        hideLeftSocketsCoroutine = StartCoroutine(HideSocketsWhenNotHovering(leftHandSockets, leftHandSocketsActions, true));
                        return;
                    }
                }

                foreach (var leftHandSocket in leftHandSockets)
                {
                    leftHandSocket.gameObject.SetActive(false);
                }
            }
            else if (handSide == HVRHandSide.Right && hideRightSocketsCoroutine == null)
            {
                bool rightHandHoverRoutinePlaying = false;
                foreach (var rightHandSocket in rightHandSockets)
                {
                    foreach (var hvrSocketHoverAction in rightHandSocket.HandGrabActions)
                    {
                        if (hvrSocketHoverAction._hoverRoutine != null)
                        {
                            rightHandHoverRoutinePlaying = true;
                            break;
                        }
                    }

                    if (rightHandSocket.IsHovering || rightHandSocket.HandHovering || rightHandHoverRoutinePlaying)
                    {
                        hideRightSocketsCoroutine = StartCoroutine(HideSocketsWhenNotHovering(rightHandSockets, rightHandSocketsActions, false));
                        return;
                    }
                }

                foreach (var rightHandSocket in rightHandSockets)
                {
                    rightHandSocket.gameObject.SetActive(false);
                }
            }
        }

        private IEnumerator HideSocketsWhenNotHovering(HVRSocket[] sockets, HVRSocketHoverScale[] socketActions, bool leftSockets)
        {
            yield return new WaitUntil(() => SocketsReadyToHide(sockets, socketActions));
            
            foreach (var socket in sockets)
            {
                socket.gameObject.SetActive(false);
            }

            if (leftSockets)
            {
                hideLeftSocketsCoroutine = null;
            }
            else
            {
                hideRightSocketsCoroutine = null;
            }
        }

        private static bool SocketsReadyToHide(HVRSocket[] sockets, HVRSocketHoverScale[] socketActions)
        {
            foreach (var socket in sockets)
            {
                if (socket.HoverTarget || socket.HandHovering)
                {
                    return false;
                }
            }

            foreach (var hvrSocketHoverAction in socketActions)
            {
                if (hvrSocketHoverAction._hoverRoutine != null)
                {
                    return false;
                }
            }

            return true;
        }

        private void HandLockTriggerEnter(HVRHandSide handSide)
        {
            if (handSide == HVRHandSide.Left)
            {
                SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, LeftHandLockTrigger.transform.position);
                if (leftHandLockTriggerHighlight)
                {
                    leftHandLockTriggerHighlight.highlighted = true;
                }
            }
            else
            {
                SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, RightHandLockTrigger.transform.position);
                if (rightHandLockTriggerHighlight)
                {
                    rightHandLockTriggerHighlight.highlighted = true;
                }
            }
        }

        private void HandLockTriggerExit(HVRHandSide handSide)
        {
            if (handSide == HVRHandSide.Left)
            {
                if (leftHandLockTriggerHighlight)
                {
                    leftHandLockTriggerHighlight.highlighted = false;
                }
            }
            else
            {
                if (rightHandLockTriggerHighlight)
                {
                    rightHandLockTriggerHighlight.highlighted = false;
                }
            }
        }

        private void HandLockTriggerExceededTime(HVRHandSide handSide)
        {
            if (handSide == HVRHandSide.Left)
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
            else
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
}