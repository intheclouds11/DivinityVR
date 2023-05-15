using System;
using System.Collections;
using System.Collections.Generic;
using HighlightPlus;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.Utils;
using HurricaneVR.Framework.Shared;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

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
        private bool inTrigger;
        private float timeInTrigger;
        private bool ExceededTimeInTrigger;
        private HVRHandSide handSide;
        private bool userIsLooking;
        private Transform cam;

        private void Awake()
        {
            var hand = GetComponentInParent<HVRHandGrabber>();
            if (hand)
            {
                handSide = hand.HandSide;
            }
            cam = LocalUserObjects.Instance.Camera.transform;
        }


        private void Update()
        {
            if (requireUserLooking)
            {
                if (Physics.SphereCast(cam.position, SphereCastRadius, cam.forward, out RaycastHit hitInfo, 1, ~LayerMask.NameToLayer("Hand"),
                        QueryTriggerInteraction.Collide))
                {
                    if (hitInfo.collider.gameObject == gameObject)
                    {
                        userIsLooking = true;
                    }
                    else
                    {
                        userIsLooking = false;
                    }
                }
                else
                {
                    userIsLooking = false;
                }
            }

            if (TagToDetect != "HandTriggerCollider")
            {
                return;
            }

            if (inTrigger)
            {
                timeInTrigger += Time.deltaTime;
            }
            else
            {
                ExceededTimeInTrigger = false;
                timeInTrigger = 0;
            }

            if (!ExceededTimeInTrigger && RequiredTimeInTrigger > 0 && timeInTrigger >= RequiredTimeInTrigger)
            {
                HandExceededRequiredTime.Invoke(handSide);
                ExceededTimeInTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (requireUserLooking && !userIsLooking)
            {
                return;
            }

            if (other.CompareTag(TagToDetect))
            {
                inTrigger = true;
                if (TagToDetect == "HandTriggerCollider")
                {
                    HandTriggerEnterEvent.Invoke(handSide);
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
                inTrigger = false;
                if (TagToDetect == "HandTriggerCollider")
                {
                    HandTriggerExitEvent.Invoke(handSide);
                }
                else
                {
                    TriggerExitEvent.Invoke(other);
                }
            }
        }
    }
}