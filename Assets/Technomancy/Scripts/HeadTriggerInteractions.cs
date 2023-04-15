using System.Collections;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Sockets;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;

namespace intheclouds
{
    public class HeadTriggerInteractions : MonoBehaviour
    {
        private PlayerStats playerStats;
        public AudioClip drinkClip;
        private Potion potion;

        private void Awake()
        {
            playerStats = transform.GetComponentInParent<PlayerStats>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Potion"))
            {
                potion = other.transform.parent.GetComponent<Potion>();
                if (!potion.Usable || potion.Used || potion.GetComponent<HVRGrabbable>().IsSocketed)
                {
                    return;
                }

                if (playerStats.Turn && playerStats.CurrentAP > potion.RequiredAP)
                {
                    playerStats.UseAP(potion.RequiredAP);
                }
                else if (playerStats.InCombat)
                {
                    SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.errorSFX, transform.position, 1, 0.5f);
                    return;
                }

                SFXPlayer.Instance.PlaySFX(drinkClip, transform.position, 1, 0.5f);
                potion.Used = true;
                Destroy(potion.GetComponent<HVRTagSocketable>());
                StartCoroutine(Drink());
            }
        }

        private IEnumerator Drink()
        {
            yield return new WaitForSeconds(0.65f);

            if (potion.Type == Potion.PotionType.Health)
            {
                playerStats.Heal(potion.Amount);
            }
            else if (potion.Type == Potion.PotionType.MagicArmor)
            {
                playerStats.RestoreMagicArmor(potion.Amount);
            }
            else if (potion.Type == Potion.PotionType.PhysicalArmor)
            {
                playerStats.RestorePhysicalArmor(potion.Amount);
            }

            Destroy(potion.gameObject);
        }
    }
}