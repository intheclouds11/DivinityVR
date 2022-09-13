using System;
using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;

namespace intheclouds
{
    public class SurfaceEffect : MonoBehaviour
    {
        public StatusEffect type;
        public int damage;
        public AudioClip damageClip;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.CompareTag("Enemy"))
            {
                Debug.Log($"{other.gameObject.name} stepped on burning surface!");
                SFXPlayer.Instance.PlaySFXAttach(damageClip, transform, 1, 1);
            }
        }
    }
}