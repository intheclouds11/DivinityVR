using System;
using System.Collections;
using System.Collections.Generic;
using HighlightPlus;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;

namespace intheclouds
{
    public class TriggerEvents : MonoBehaviour
    {
        public float RequiredTimeInTrigger = 1.5f;
        public bool ExceededTimeInTrigger;
        public event Action ExceededTimeInTriggerAction;
        private float timeInTrigger;
        private bool inTrigger;
        private HVRHandGrabber handGrabber;
        private HighlightEffect highlightEffect;

        private void Awake()
        {
            handGrabber = GetComponentInParent<HVRHandGrabber>();
            highlightEffect = GetComponent<HighlightEffect>();
        }

        private void Update()
        {
            if (inTrigger)
            {
                timeInTrigger += Time.deltaTime;
            }
            else
            {
                ExceededTimeInTrigger = false;
                timeInTrigger = 0;
            }
            
            if (!ExceededTimeInTrigger && timeInTrigger >= RequiredTimeInTrigger)
            {
                ExceededTimeInTriggerAction?.Invoke();
                ExceededTimeInTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("HandTriggerCollider") && handGrabber.GrabbedTarget)
            {
                highlightEffect.highlighted = true;
                inTrigger = true;
                SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.clickSFX, transform.position);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("HandTriggerCollider"))
            {
                highlightEffect.highlighted = false;
                inTrigger = false;
            }
        }
    }
}
