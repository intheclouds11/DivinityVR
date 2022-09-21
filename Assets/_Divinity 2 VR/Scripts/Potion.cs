using System;
using System.Collections;
using System.Collections.Generic;
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