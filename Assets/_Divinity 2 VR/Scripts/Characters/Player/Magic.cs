using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class Magic : MonoBehaviour
    {
        public GameObject skillDescription;
        public int cooldown;
        public int cooldownTimer;
        public MagicSystem magicSystem;
        public int baseDamage;
        public int requiredAP;
        public GameObject impactVFX;
        public AudioClip noDamageAudioClip;
        public GameObject surfaceEffect;
        public PlayerStats caster;
    }
}
