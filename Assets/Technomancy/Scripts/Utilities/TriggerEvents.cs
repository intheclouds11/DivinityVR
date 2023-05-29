using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.Utils;
using HurricaneVR.Framework.Shared;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace intheclouds
{
    public class TriggerEvents : MonoBehaviour
    {
        public bool requireUserLooking;
        [DrawIf(nameof(requireUserLooking), true)]
        public float SphereCastRadius = 0.15f;
        [Tag]
        public string TagToDetect = "HandTriggerCollider";
        public float RequiredTimeInTrigger = 1.5f;
        public UnityEvent<HVRHandSide> HandExceededRequiredTime;
        public UnityEvent<HVRHandSide> HandTriggerEnterEvent;
        public UnityEvent<HVRHandSide> HandTriggerExitEvent;
        public UnityEvent<Collider> ExceededRequiredTime;
        public UnityEvent<Collider> TriggerEnterEvent;
        public UnityEvent<Collider> TriggerExitEvent;
        private bool _inTrigger;
        private float _timeInTrigger;
        private bool _exceededTimeInTrigger;
        private HVRHandSide _handSide;
        private bool _userIsLooking;
        private Transform _cam;

        private void Awake()
        {
            var hand = GetComponentInParent<HVRHandGrabber>();
            if (hand)
            {
                _handSide = hand.HandSide;
            }
            _cam = LocalUserObjects.instance.Camera.transform;
        }


        private void Update()
        {
            if (requireUserLooking)
            {
                if (Physics.SphereCast(_cam.position, SphereCastRadius, _cam.forward, out RaycastHit hitInfo, 1, ~LayerMask.NameToLayer("Hand"),
                        QueryTriggerInteraction.Collide))
                {
                    if (hitInfo.collider.gameObject == gameObject)
                    {
                        _userIsLooking = true;
                    }
                    else
                    {
                        _userIsLooking = false;
                    }
                }
                else
                {
                    _userIsLooking = false;
                }
            }

            if (TagToDetect != "HandTriggerCollider")
            {
                return;
            }

            if (_inTrigger)
            {
                _timeInTrigger += Time.deltaTime;
            }
            else
            {
                _exceededTimeInTrigger = false;
                _timeInTrigger = 0;
            }

            if (!_exceededTimeInTrigger && RequiredTimeInTrigger > 0 && _timeInTrigger >= RequiredTimeInTrigger)
            {
                HandExceededRequiredTime.Invoke(_handSide);
                _exceededTimeInTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (requireUserLooking && !_userIsLooking)
            {
                return;
            }

            if (other.CompareTag(TagToDetect))
            {
                _inTrigger = true;
                if (TagToDetect == "HandTriggerCollider")
                {
                    HandTriggerEnterEvent.Invoke(_handSide);
                }
                else
                {
                    TriggerEnterEvent.Invoke(other);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(TagToDetect))
            {
                _inTrigger = false;
                if (TagToDetect == "HandTriggerCollider")
                {
                    HandTriggerExitEvent.Invoke(_handSide);
                }
                else
                {
                    TriggerExitEvent.Invoke(other);
                }
            }
        }
    }
}