using System;
using HurricaneVR.Framework.Shared;
using UnityEngine;

namespace intheclouds
{
    public class All_In : AbilityBase
    {
        private Sword weapon;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Sword"))
            {
                
            }
        }

        private void Update()
        {
            if (weapon != null && weapon.hitEnemyCollider.transform.gameObject.activeSelf)
            {
                Activate();
                OnAbilityUsed();
                ResetAbilityTransform();
                castingHand.GrabTrigger = HVRGrabTrigger.Active;
                weapon.baseDamage = (int) Math.Ceiling(castingHand.GrabbedTarget.GetComponent<Sword>().baseDamage * 0.8f);
                weapon = null;
            }
        }

        private void Activate()
        {
            if (castingHand.Controller.Side == HVRHandSide.Left)
            {
                weapon = abilitySystem.rightHandGrabber.GrabbedTarget.GetComponent<Sword>();
            }
            else if (castingHand.Controller.Side == HVRHandSide.Right)
            {
                weapon = abilitySystem.leftHandGrabber.GrabbedTarget.GetComponent<Sword>();
            }

            castingHand.GrabTrigger = HVRGrabTrigger.ManualRelease;
            weapon.baseDamage = (int) Math.Ceiling(castingHand.GrabbedTarget.GetComponent<Sword>().baseDamage * 1.25f);

            if (activatedVFX != null)
            {
                activatedVFX.transform.parent = null;
                activatedVFX.transform.position = weapon.transform.position;
                activatedVFX.transform.eulerAngles = weapon.transform.eulerAngles;
                activatedVFX.SetActive(true);
            }
        }
    }
}