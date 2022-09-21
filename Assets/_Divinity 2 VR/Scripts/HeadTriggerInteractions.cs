using System;
using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.Core.Sockets;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace intheclouds
{
    public class HeadTriggerInteractions : MonoBehaviour
    {
        private PlayerStats playerStats;
        public AudioClip drinkClip;
        private Potion potion;

        private void Awake()
        {
            playerStats = transform.root.GetComponent<PlayerStats>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Potion"))
            {
                SFXPlayer.Instance.PlaySFXAttach(drinkClip, transform, 1, 1);
                other.tag = "Untagged";
                potion = other.transform.parent.GetComponent<Potion>();
                Destroy(potion.GetComponent<HVRTagSocketable>());
                StartCoroutine(Drink(other));
            }
        }

        private IEnumerator Drink(Collider other)
        {
            yield return new WaitForSeconds(0.65f);

            if (potion.type == Potion.PotionType.Health)
            {
                playerStats.Heal(potion.amount);
            }
            else if (potion.type == Potion.PotionType.MagicArmor)
            {
                playerStats.RestoreMagicArmor(potion.amount);
            }
            else if (potion.type == Potion.PotionType.PhysicalArmor)
            {
                playerStats.RestorePhysicalArmor(potion.amount);
            }

            Destroy(other.transform.parent.gameObject);
        }
    }
}