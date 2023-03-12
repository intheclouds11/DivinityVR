using System.Collections;
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

                if (playerStats.Turn && playerStats.CurrentAP > potion.requiredAP)
                {
                    playerStats.UseAP(potion.requiredAP);
                }
                else if (playerStats.InCombat)
                {
                    SFXPlayer.Instance.PlaySFX(SFXPlayer.Instance.errorSFX, transform.position, 1, 1);
                    return;
                }

                SFXPlayer.Instance.PlaySFX(drinkClip, transform.position, 1, 1);
                other.tag = "Untagged";
                Destroy(potion.GetComponent<HVRTagSocketable>());
                StartCoroutine(Drink());
            }
        }

        private IEnumerator Drink()
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

            Destroy(potion.gameObject);
        }
    }
}