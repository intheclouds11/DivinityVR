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
        public static UserInventory instance;
        public HVRHandGrabber leftHandGrabber;
        public HVRHandGrabber rightHandGrabber;
        public TriggerEvents leftHandLockTrigger;
        public TriggerEvents rightHandLockTrigger;
        public TriggerEvents leftHandSocketsTrigger;
        public TriggerEvents rightHandSocketsTrigger;
        
        private HVRSocket[] _leftHandSockets;
        private HVRSocket[] _rightHandSockets;
        private HVRSocketHoverScale[] _leftHandSocketsActions;
        private HVRSocketHoverScale[] _rightHandSocketsActions;
        private HighlightEffect _leftHandLockTriggerHighlight;
        private HighlightEffect _rightHandLockTriggerHighlight;
        private Coroutine _hideLeftSocketsCoroutine;
        private Coroutine _hideRightSocketsCoroutine;
        private bool _leftHandCanEquip;
        private bool _rightHandCanEquip;
        private ITCHandGrabber[] _handGrabbers;

        private void Awake()
        {
            instance = this;
            _handGrabbers = new[] {leftHandGrabber as ITCHandGrabber, rightHandGrabber as ITCHandGrabber};

            _leftHandSockets = leftHandSocketsTrigger.GetComponentsInChildren<HVRSocket>(true);
            _rightHandSockets = rightHandSocketsTrigger.GetComponentsInChildren<HVRSocket>(true);
            _leftHandSocketsActions = leftHandSocketsTrigger.GetComponentsInChildren<HVRSocketHoverScale>(true);
            _rightHandSocketsActions = rightHandSocketsTrigger.GetComponentsInChildren<HVRSocketHoverScale>(true);
            _leftHandLockTriggerHighlight = leftHandLockTrigger.GetComponent<HighlightEffect>();
            _rightHandLockTriggerHighlight = rightHandLockTrigger.GetComponent<HighlightEffect>();

            leftHandSocketsTrigger.HandTriggerEnterEvent.AddListener(HandInventoryTriggerEnter);
            leftHandSocketsTrigger.HandTriggerExitEvent.AddListener(HandInventoryTriggerExit);
            rightHandSocketsTrigger.HandTriggerEnterEvent.AddListener(HandInventoryTriggerEnter);
            rightHandSocketsTrigger.HandTriggerExitEvent.AddListener(HandInventoryTriggerExit);
            leftHandLockTrigger.HandExceededRequiredTime.AddListener(HandLockTriggerExceededTime);
            leftHandLockTrigger.HandTriggerEnterEvent.AddListener(HandLockTriggerEnter);
            leftHandLockTrigger.HandTriggerExitEvent.AddListener(HandLockTriggerExit);
            rightHandLockTrigger.HandExceededRequiredTime.AddListener(HandLockTriggerExceededTime);
            rightHandLockTrigger.HandTriggerEnterEvent.AddListener(HandLockTriggerEnter);
            rightHandLockTrigger.HandTriggerExitEvent.AddListener(HandLockTriggerExit);
        }

        public bool IsHoldingWeapon()
        {
            foreach (var handGrabber in _handGrabbers)
            {
                if (!handGrabber.GrabbedTarget) continue;

                handGrabber.GrabbedTarget.TryGetComponent(out ImpactHandler weapon);
                if (weapon) return true;

                break;
            }

            return false;
        }

        private void HandInventoryTriggerEnter(HVRHandSide handSide)
        {
            if (handSide == HVRHandSide.Left)
            {
                if (rightHandGrabber.IsHovering || rightHandGrabber.ForceGrabber.IsHovering) return;
                foreach (var leftHandSocket in _leftHandSockets)
                {
                    leftHandSocket.gameObject.SetActive(true);
                }
            }
            else
            {
                if (leftHandGrabber.IsHovering || leftHandGrabber.ForceGrabber.IsHovering) return;
                foreach (var rightHandSocket in _rightHandSockets)
                {
                    rightHandSocket.gameObject.SetActive(true);
                }
            }
        }

        private void HandInventoryTriggerExit(HVRHandSide handSide)
        {
            if (handSide == HVRHandSide.Left && _hideLeftSocketsCoroutine == null)
            {
                TryHideHandSockets(_leftHandSockets, _leftHandSocketsActions, true);
            }
            else if (handSide == HVRHandSide.Right && _hideRightSocketsCoroutine == null)
            {
                TryHideHandSockets(_rightHandSockets, _rightHandSocketsActions, false);
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
                        _hideLeftSocketsCoroutine = StartCoroutine(HideSocketsWhenNotHovering(sockets, socketActions, true));
                    }
                    else
                    {
                        _hideRightSocketsCoroutine = StartCoroutine(HideSocketsWhenNotHovering(sockets, socketActions, false));
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
                _hideLeftSocketsCoroutine = null;
            }
            else
            {
                _hideRightSocketsCoroutine = null;
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
            if (handSide == HVRHandSide.Left)
            {
                _leftHandCanEquip = leftHandGrabber.GrabbedTarget && leftHandGrabber.GrabbedTarget.Socketable;
                if (!_leftHandCanEquip) return;
                SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, leftHandLockTrigger.transform.position);
                if (_leftHandLockTriggerHighlight)
                {
                    _leftHandLockTriggerHighlight.highlighted = true;
                }
            }
            else if (handSide == HVRHandSide.Right)
            {
                _rightHandCanEquip = rightHandGrabber.GrabbedTarget && rightHandGrabber.GrabbedTarget.Socketable;
                if (!_rightHandCanEquip) return;
                SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, rightHandLockTrigger.transform.position);
                if (_rightHandLockTriggerHighlight)
                {
                    _rightHandLockTriggerHighlight.highlighted = true;
                }
            }
        }

        private void HandLockTriggerExceededTime(HVRHandSide handSide)
        {
            if (handSide == HVRHandSide.Left)
            {
                if (leftHandGrabber.GrabTrigger != HVRGrabTrigger.ManualRelease && _leftHandCanEquip)
                {
                    leftHandGrabber.GrabTrigger = HVRGrabTrigger.ManualRelease;
                    leftHandGrabber.GrabbedTarget.CanBeGrabbed = false;
                    SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, LocalUserObjects.instance.Camera.transform.position, 1.2f, 1);
                    Debug.Log($"Equipped: {leftHandGrabber.GrabbedTarget}");
                }
                else if (leftHandGrabber.GrabTrigger == HVRGrabTrigger.ManualRelease)
                {
                    leftHandGrabber.GrabTrigger = HVRGrabTrigger.Toggle;
                    leftHandGrabber.GrabbedTarget.CanBeGrabbed = true;
                    _leftHandCanEquip = false;
                    SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, LocalUserObjects.instance.Camera.transform.position, 0.8f, 1);
                    Debug.Log($"Dequipped: {leftHandGrabber.GrabbedTarget}");
                }
            }
            else
            {
                if (rightHandGrabber.GrabTrigger != HVRGrabTrigger.ManualRelease && _rightHandCanEquip)
                {
                    rightHandGrabber.GrabTrigger = HVRGrabTrigger.ManualRelease;
                    rightHandGrabber.GrabbedTarget.CanBeGrabbed = false;
                    SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, LocalUserObjects.instance.Camera.transform.position, 1.2f, 1);
                    Debug.Log($"Equipped: {rightHandGrabber.GrabbedTarget}");
                }
                else if (rightHandGrabber.GrabTrigger == HVRGrabTrigger.ManualRelease)
                {
                    rightHandGrabber.GrabTrigger = HVRGrabTrigger.Toggle;
                    rightHandGrabber.GrabbedTarget.CanBeGrabbed = true;
                    _rightHandCanEquip = false;
                    SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, LocalUserObjects.instance.Camera.transform.position, 0.8f, 1);
                    Debug.Log($"Dequipped: {rightHandGrabber.GrabbedTarget}");
                }
            }
        }

        private void HandLockTriggerExit(HVRHandSide handSide)
        {
            if (handSide == HVRHandSide.Left)
            {
                if (_leftHandLockTriggerHighlight && leftHandGrabber.GrabTrigger != HVRGrabTrigger.ManualRelease)
                {
                    _leftHandLockTriggerHighlight.highlighted = false;
                }
            }
            else
            {
                if (_rightHandLockTriggerHighlight && rightHandGrabber.GrabTrigger != HVRGrabTrigger.ManualRelease)
                {
                    _rightHandLockTriggerHighlight.highlighted = false;
                }
            }
        }
    }
}