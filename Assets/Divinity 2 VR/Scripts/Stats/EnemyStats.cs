using System;
using HurricaneVR.Framework.Core.Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace intheclouds
{
    public class EnemyStats : MonoBehaviour
    {
        public EnemyStatsSO enemyStatsSO;
        public Slider healthSlider;
        public Slider physicalArmorSlider;
        public Slider magicArmorSlider;
        public TextMeshProUGUI healthText;
        public TextMeshProUGUI physicalArmorText;
        public TextMeshProUGUI magicArmorText;
        public int currentHealth;
        public int maxHealth;
        public int currentPhysicalArmor;
        public int maxPhysicalArmor;
        public int currentMagicArmor;
        public int maxMagicArmor;
        public int currentAP;
        public int maxAP;
        public int earnedXP;

        private AudioSource audioSource;
        public AudioClip[] audioClips;
        public event Action Damaged; // use for other classes to know when player is damaged

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            maxHealth = enemyStatsSO.maxHealth;
            currentHealth = maxHealth;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
            healthText.text = $"{currentHealth}/{maxHealth}";

            maxPhysicalArmor = enemyStatsSO.maxPhysicalArmor;
            currentPhysicalArmor = maxPhysicalArmor;
            physicalArmorSlider.maxValue = maxPhysicalArmor;
            physicalArmorSlider.value = currentPhysicalArmor;
            physicalArmorText.text = $"{currentPhysicalArmor}/{maxPhysicalArmor}";

            maxMagicArmor = enemyStatsSO.maxMagicArmor;
            currentMagicArmor = maxMagicArmor;
            magicArmorSlider.maxValue = maxMagicArmor;
            magicArmorSlider.value = currentMagicArmor;
            magicArmorText.text = $"{currentMagicArmor}/{maxMagicArmor}";

            maxAP = enemyStatsSO.maxAP;
            currentAP = maxAP;
            earnedXP = enemyStatsSO.earnedXP;
        }

        private void OnEnable()
        {
            this.Damaged += DamageEventExample;
        }

        private void OnDisable()
        {
            this.Damaged -= DamageEventExample;
        }

        public void TakeDamage(int damage) // todo: add type of damage
        {
            // audioSource.pitch = Random.Range( )
            audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
            Damaged?.Invoke();
            currentHealth -= damage;
            healthSlider.value = currentHealth;
            healthText.text = $"{currentHealth}/{maxHealth}";

            if (currentHealth <= 0)
            {
                // ragdoll death
            }
        }

        public void DamageEventExample()
        {
            Debug.Log("Enemy damaged event example");
        }
    }
}