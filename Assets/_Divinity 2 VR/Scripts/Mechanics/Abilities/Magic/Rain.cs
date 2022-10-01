using System;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class Rain : AbilityBase
    {
        public Rigidbody rb;
        public AudioClip activatedClip;
        public float maxDistance = 2;
        private Vector3 targetLocation;
        private int layerMask;
        public GameObject markerObj;

        protected override void OnEnable()
        {
            markerObj.SetActive(true);
            markerObj.transform.parent = null;
            markerObj.transform.rotation = Quaternion.identity;
            base.OnEnable();
        }

        private void Start()
        {
            layerMask = ~LayerMask.NameToLayer("Ignore Raycast");
        }

        private void Update()
        {
            if (Physics.Raycast(caster.LocalUserObjects.Camera.transform.position, caster.LocalUserObjects.Camera.transform.forward, out RaycastHit hit, maxDistance,
                    layerMask, QueryTriggerInteraction.Ignore))
            {
                targetLocation = hit.point;
                markerObj.transform.position = targetLocation;
            }

            if (rb.velocity.y < -2f)
            {
                Activate();
            }
        }

        public void Activate()
        {
            markerObj.SetActive(false);
            markerObj.transform.parent = transform;
            activatedVFX.transform.position = targetLocation;
            activatedVFX.transform.eulerAngles = new Vector3(-90, 0, 0);
            SFXPlayer.Instance.PlaySFX(activatedClip, transform.position);
            OnAbilityUsed();
        }
    }
}