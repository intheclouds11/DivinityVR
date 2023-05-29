using System.Collections;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class Potion : MonoBehaviour, IHoverableItem
    {
        public enum PotionType
        {
            Health,
            MagicArmor,
            PhysicalArmor
        }

        public PotionType type;
        public int amount = 25;
        public Collider grabbableCollider;
        public int requiredAP = 1;
        public AudioClip drinkClip;
        public GameObject liquidGO;
        
        public ITCGrabbable grabbable { get; private set; }
        public bool Used { get; set; }
        public bool Usable { get; set; }
        

        private void Awake()
        {
            grabbable = GetComponent<ITCGrabbable>();
        }

        public void ToggleTagOnSocketed()
        {
            if (grabbableCollider.CompareTag("Potion"))
            {
                grabbableCollider.tag = "Untagged";
            }
            else
            {
                grabbableCollider.tag = "Potion";
            }
        }

        public void StartDrinkCoroutine(PlayerStats playerStats)
        {
            StartCoroutine(Drink(playerStats));
        }
        
        public IEnumerator Drink(PlayerStats playerStats)
        {
            Used = true;
            SFXPlayer.Instance.PlaySFX(drinkClip, transform.position, 1, 0.5f, 10, false);

            yield return new WaitForSeconds(0.65f);

            if (type == PotionType.Health)
            {
                playerStats.Heal(amount);
            }
            else if (type == PotionType.MagicArmor)
            {
                playerStats.RestoreMagicArmor(amount);
            }
            else if (type == PotionType.PhysicalArmor)
            {
                playerStats.RestorePhysicalArmor(amount);
            }

            liquidGO.SetActive(false);
        }

        public string GetHoverInfo()
        {
            return $"{type} Potion: +{amount}";
        }
    }
}