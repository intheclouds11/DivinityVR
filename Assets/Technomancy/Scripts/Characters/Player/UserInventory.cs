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
            if (handSide == HVRHandSide.Left)
            {
                if (RightHandGrabber.IsHovering || RightHandGrabber.ForceGrabber.IsHovering) return;
                foreach (var leftHandSocket in leftHandSockets)
                {
                    leftHandSocket.gameObject.SetActive(true);
                }
            }
            else
            {
                if (LeftHandGrabber.IsHovering || LeftHandGrabber.ForceGrabber.IsHovering) return;
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
                TryHideHandSockets(leftHandSockets, leftHandSocketsActions, true);
            }
            else if (handSide == HVRHandSide.Right && hideRightSocketsCoroutine == null)
            {
                TryHideHandSockets(rightHandSockets, rightHandSocketsActions, false);
            }
        }

        private void TryHideHandSockets(HVRSocket[] sockets, HVRSocketHoverScale[] socketActions, bool leftSockets)
        {
            foreach (var socket in sockets)
            {
                if (socket.IsHovering || socket.HandHovering || socket.HandGrabActions.Any(hvrSocketHoverAction => hvrSocketHoverAction._hoverRoutine != null))
                {
                    if (leftSockets)
                    {
                        hideLeftSocketsCoroutine = StartCoroutine(HideSocketsWhenNotHovering(sockets, socketActions, true));
                    }
                    else
                    {
                        hideRightSocketsCoroutine = StartCoroutine(HideSocketsWhenNotHovering(sockets, socketActions, false));
                    }

                    return;
                }
            }

            foreach (var socket in sockets)
            {
                socket.gameObject.SetActive(false);
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
            if (sockets.Any(socket => socket.IsHovering || socket.HandHovering))
            {
                return false;
            }

            return socketActions.All(hvrSocketHoverAction => hvrSocketHoverAction._hoverRoutine == null);
        }

        private void HandLockTriggerEnter(HVRHandSide handSide)
        {
            if (handSide == HVRHandSide.Left && LeftHandGrabber.GrabbedTarget && LeftHandGrabber.GrabbedTarget.Socketable)
            {
                SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, LeftHandLockTrigger.transform.position);
                if (leftHandLockTriggerHighlight)
                {
                    leftHandLockTriggerHighlight.highlighted = true;
                }
            }
            else if (handSide == HVRHandSide.Right && RightHandGrabber.GrabbedTarget && RightHandGrabber.GrabbedTarget.Socketable)
            {
                SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, RightHandLockTrigger.transform.position);
                if (rightHandLockTriggerHighlight)
                {
                    rightHandLockTriggerHighlight.highlighted = true;
                }
            }
        }

        private void HandLockTriggerExceededTime(HVRHandSide handSide)
        {
            if (handSide == HVRHandSide.Left)
            {
                if (LeftHandGrabber.GrabTrigger != HVRGrabTrigger.ManualRelease && LeftHandGrabber.GrabbedTarget.Socketable)
                {
                    LeftHandGrabber.GrabTrigger = HVRGrabTrigger.ManualRelease;
                    LeftHandGrabber.GrabbedTarget.CanBeGrabbed = false;
                    SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, LocalUserObjects.Instance.Camera.transform.position, 1.2f, 1);
                    Debug.Log($"Equipped: {LeftHandGrabber.GrabbedTarget}");
                }
                else if (LeftHandGrabber.GrabTrigger == HVRGrabTrigger.ManualRelease)
                {
                    LeftHandGrabber.GrabTrigger = HVRGrabTrigger.Toggle;
                    LeftHandGrabber.GrabbedTarget.CanBeGrabbed = true;
                    SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, LocalUserObjects.Instance.Camera.transform.position, 0.8f, 1);
                    Debug.Log($"Dequipped: {LeftHandGrabber.GrabbedTarget}");
                }
            }
            else
            {
                if (RightHandGrabber.GrabTrigger != HVRGrabTrigger.ManualRelease && RightHandGrabber.GrabbedTarget.Socketable)
                {
                    RightHandGrabber.GrabTrigger = HVRGrabTrigger.ManualRelease;
                    RightHandGrabber.GrabbedTarget.CanBeGrabbed = false;
                    SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, LocalUserObjects.Instance.Camera.transform.position, 1.2f, 1);
                    Debug.Log($"Equipped: {RightHandGrabber.GrabbedTarget}");
                }
                else if (RightHandGrabber.GrabTrigger == HVRGrabTrigger.ManualRelease)
                {
                    RightHandGrabber.GrabTrigger = HVRGrabTrigger.Toggle;
                    RightHandGrabber.GrabbedTarget.CanBeGrabbed = true;
                    SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, LocalUserObjects.Instance.Camera.transform.position, 0.8f, 1);
                    Debug.Log($"Dequipped: {RightHandGrabber.GrabbedTarget}");
                }
            }
        }

        private void HandLockTriggerExit(HVRHandSide handSide)
        {
            if (handSide == HVRHandSide.Left)
            {
                if (leftHandLockTriggerHighlight && LeftHandGrabber.GrabTrigger != HVRGrabTrigger.ManualRelease)
                {
                    leftHandLockTriggerHighlight.highlighted = false;
                }
            }
            else
            {
                if (rightHandLockTriggerHighlight && RightHandGrabber.GrabTrigger != HVRGrabTrigger.ManualRelease)
                {
                    rightHandLockTriggerHighlight.highlighted = false;
                }
            }
        }
    }
}