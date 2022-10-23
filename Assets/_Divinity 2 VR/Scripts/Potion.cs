using UnityEngine;

namespace intheclouds
{
    public class Potion : MonoBehaviour
    {
        public enum PotionType
        {
            Health,
            MagicArmor,
            PhysicalArmor
        }

        public PotionType type;
        public int amount;
        public Collider col;
        public int requiredAP;

        public void ToggleTagOnSocketed()
        {
            if (!enabled)
            {
                return;
            }

            if (col.CompareTag("Potion"))
            {
                col.tag = "Untagged";
            }
            else
            {
                col.tag = "Potion";
            }
        }
    }
}