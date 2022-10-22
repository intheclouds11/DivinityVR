using System;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class Rain : AbilityBase
    {
        public Rigidbody rb;
        
        private void Update()
        {
            if (rb.velocity.y < -2f && AbilitySpawnLocator.Instance.IsSelectionValid)
            {
                Activate(AbilitySpawnLocator.Instance.SelectionLocation, new Vector3(-90, 0, 0));
            }
        }
        
        private void Activate(Vector3 position, Vector3 rotation)
        {
            if (activatedVFX != null)
            {
                activatedVFX.transform.parent = null;
                activatedVFX.transform.position = position;
                activatedVFX.transform.eulerAngles = rotation;
                activatedVFX.SetActive(true);
            }

            if (casterVFX != null)
            {
                casterVFX.SetActive(true);
            }

            OnAbilityUsed();
            ResetAbilityTransform();
        }
    }
}