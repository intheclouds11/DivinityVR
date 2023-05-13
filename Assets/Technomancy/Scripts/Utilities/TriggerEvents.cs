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
        public UnityEvent<HVRHandSide> ExceededRequiredTime;
        public UnityEvent<HVRHandSide> TriggerEnterEvent;
        public UnityEvent<HVRHandSide> TriggerExitEvent;
        private bool inTrigger;
        private float timeInTrigger;
        private bool ExceededTimeInTrigger;
        private HVRHandGrabber thisHand;
        private bool userIsLooking;
        private Transform cam;

        private void Awake()
        {
            thisHand = GetComponentInParent<HVRHandGrabber>();
            cam = LocalUserObjects.Instance.Camera.transform;
        }


        private void Update()
        {
            if (requireUserLooking)
            {
                if (Physics.SphereCast(cam.position, SphereCastRadius, cam.forward, out RaycastHit hitInfo, 1, ~LayerMask.NameToLayer("Hand"), QueryTriggerInteraction.Collide))
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
                ExceededRequiredTime.Invoke(thisHand.HandSide);
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
                TriggerEnterEvent.Invoke(thisHand.HandSide);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(TagToDetect))
            {
                inTrigger = false;
                TriggerExitEvent.Invoke(thisHand.HandSide);
            }
        }
    }
}
