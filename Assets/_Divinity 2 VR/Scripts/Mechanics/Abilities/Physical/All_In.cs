using System;
using HighlightPlus;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.Utils;
using HurricaneVR.Framework.Shared;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

namespace intheclouds
{
    public class All_In : AbilityBase
    {
        public AudioClip appliedSFX;
        private Sword weapon;
        private HighlightEffect handHighlight;

        protected override void OnEnable()
        {
            handHighlight = castingHand.gameObject.GetComponent<HighlightEffect>();
            handHighlight.enabled = true;
            base.OnEnable();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Sword"))
            {
                weapon = other.gameObject.GetComponentInParent<Sword>();

                if (weapon != null)
                {
                    ApplyToHeldWeapon();
                    weapon.SwordAppliedDamage += OnSwordAppliedDamage;
                }
                else
                {
                    Debug.LogError("All In couldn't find weapon script");
                }
            }
        }
        
        private void OnSwordAppliedDamage()
        {
            weapon.SwordAppliedDamage -= OnSwordAppliedDamage;
            HVRHandGrabber grabber = (HVRHandGrabber) weapon.GetComponent<HVRGrabbable>().PrimaryGrabber;
            grabber.GrabTrigger = HVRGrabTrigger.Active;
            weapon.baseDamage = (int) Math.Floor(weapon.baseDamage * 0.8f);
            weapon.GetComponent<HighlightEffect>().enabled = false;
            weapon = null;
            GetComponent<BoxCollider>().enabled = true;
            OnAbilityUsed();
            ResetAbilityTransform();
        }

        private void ApplyToHeldWeapon()
        {
            SFXPlayer.Instance.PlaySFX(appliedSFX, weapon.transform.position, 0.8f, 20);
            GetComponent<BoxCollider>().enabled = false;
            
            HVRHandGrabber grabber = (HVRHandGrabber) weapon.GetComponent<HVRGrabbable>().PrimaryGrabber;
            grabber.GrabTrigger = HVRGrabTrigger.ManualRelease;
            weapon.baseDamage = (int) Math.Ceiling(weapon.baseDamage * 1.25f);
            weapon.GetComponent<HighlightEffect>().enabled = true;
            
            handHighlight.enabled = false;

            if (activatedVFX != null)
            {
                activatedVFX.transform.parent = weapon.gameObject.transform;
                activatedVFX.transform.localPosition = Vector3.zero;
                activatedVFX.transform.localRotation = Quaternion.identity;
                activatedVFX.SetActive(true);
            }
        }
    }
}