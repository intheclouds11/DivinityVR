using UnityEngine;
using UnityEngine.Serialization;

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

        public PotionType Type;
        public int Amount = 25;
        public Collider col;
        public int RequiredAP = 1;
        [field: SerializeField]
        public bool Used { get; set; }
        [field: SerializeField]
        public bool Usable { get; set; }

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