using System;
using HighlightPlus;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.Utils;
using HurricaneVR.Framework.Shared;
using UnityEngine;

namespace intheclouds
{
    public class AllIn : AbilityBase
    {
        public AudioClip appliedSFX;
        private ImpactHandler _weapon;
        private HighlightEffect _handHighlight;

        protected override void OnEnable()
        {
            _handHighlight = castingHand.gameObject.GetComponent<HighlightEffect>();
            _handHighlight.enabled = true;
            base.OnEnable();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Sword"))
            {
                _weapon = other.gameObject.GetComponentInParent<ImpactHandler>();

                if (_weapon != null)
                {
                    ApplyToHeldWeapon();
                    _weapon.AppliedDamage += OnSwordAppliedDamage;
                }
                else
                {
                    Debug.LogError("All In couldn't find weapon script");
                }
            }
        }
        
        private void OnSwordAppliedDamage()
        {
            _weapon.AppliedDamage -= OnSwordAppliedDamage;
            HVRHandGrabber grabber = (HVRHandGrabber) _weapon.GetComponent<HVRGrabbable>().PrimaryGrabber;
            grabber.GrabTrigger = HVRGrabTrigger.Active;
            _weapon.GetComponent<HVRGrabbable>().CanBeGrabbed = true;
            _weapon.baseDamage -= (int) Math.Floor(_weapon.baseDamage * 0.25f);
            _weapon.GetComponent<HighlightEffect>().enabled = false;
            _weapon = null;
            GetComponent<BoxCollider>().enabled = true;
            OnAbilityUsed();
            ResetAbilityTransform();
        }

        private void ApplyToHeldWeapon()
        {
            SFXPlayer.Instance.PlaySFX(appliedSFX, _weapon.transform.position, 1f, 1f);
            GetComponent<BoxCollider>().enabled = false;
            
            HVRHandGrabber grabber = (HVRHandGrabber) _weapon.GetComponent<HVRGrabbable>().PrimaryGrabber;
            _weapon.GetComponent<HVRGrabbable>().CanBeGrabbed = false;
            grabber.GrabTrigger = HVRGrabTrigger.ManualRelease;
            _weapon.baseDamage += (int) Math.Ceiling(_weapon.baseDamage * 0.25f);
            _weapon.GetComponent<HighlightEffect>().enabled = true;
            
            _handHighlight.enabled = false;

            if (activatedVFX != null)
            {
                activatedVFX.transform.parent = _weapon.gameObject.transform;
                activatedVFX.transform.localPosition = Vector3.zero;
                activatedVFX.transform.localRotation = Quaternion.identity;
                activatedVFX.SetActive(true);
            }
        }
    }
}