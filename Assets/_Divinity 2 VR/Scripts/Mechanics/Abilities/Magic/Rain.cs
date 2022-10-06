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
        // private Vector3 targetLocation;
        // private int layerMask;

        private void Start()
        {
            // layerMask = ~LayerMask.NameToLayer("Ignore Raycast");
        }

        private void Update()
        {
            // if (Physics.Raycast(caster.LocalUserObjects.Camera.transform.position, caster.LocalUserObjects.Camera.transform.forward, out RaycastHit hit, maxDistance,
            //         layerMask, QueryTriggerInteraction.Ignore))
            // {
            //     targetLocation = hit.point;
            // }

            if (rb.velocity.y < -2f && SelectionPointer.Instance.IsSelectionValid)
            {
                Activate(SelectionPointer.Instance.SelectionLocation);
            }
        }

        public void Activate(Vector3 location)
        {
            activatedVFX.transform.position = location;
            activatedVFX.transform.eulerAngles = new Vector3(-90, 0, 0);
            SFXPlayer.Instance.PlaySFX(activatedClip, transform.position);
            OnAbilityUsed();
        }
    }
}