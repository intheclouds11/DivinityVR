using System;
using System.Collections.Generic;
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
        public GameObject hitPopupPrefab;
        public GameObject hitPopupsParent;
        public float hitPopupSpeed = 0.5f;
        public float hitPopupTimer = 2;
        private List<GameObject> activeHitPopups = new List<GameObject>();
        public int currentHealth;
        public int maxHealth;
        public int currentPhysicalArmor;
        public int maxPhysicalArmor;
        public int currentMagicArmor;
        public int maxMagicArmor;
        public int currentAP;
        public int maxAP;
        public int earnedXP;
        public bool isAlive = true;

        private AudioSource audioSource;
        public AudioClip[] hurtAudioClips;
        public AudioClip[] deadAudioClips;
        public event Action Damaged; // use for other classes to know when player is damaged

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (!enemyStatsSO) return;
            maxHealth = enemyStatsSO.maxHealth;
            currentHealth = enemyStatsSO.currentHealth;
            maxPhysicalArmor = enemyStatsSO.maxPhysicalArmor;
            currentPhysicalArmor = enemyStatsSO.currentPhysicalArmor;
            maxMagicArmor = enemyStatsSO.maxMagicArmor;
            currentMagicArmor = enemyStatsSO.currentMagicArmor;

            maxAP = enemyStatsSO.maxAP;
            currentAP = enemyStatsSO.currentAP;
            earnedXP = enemyStatsSO.earnedXP;
        }

        private void Update()
        {
            if (activeHitPopups.Count > 0)
            {
                foreach (var activeHitPopup in activeHitPopups)
                {
                    if (activeHitPopup)
                    {
                        activeHitPopup.transform.Translate(0, hitPopupSpeed * Time.deltaTime, 0);
                        activeHitPopup.transform.localScale +=
                            (activeHitPopup.transform.localScale + new Vector3(0.001f, 0.001f, 0.001f)) * Time.deltaTime;
                    }
                }
            }

            if (!isAlive) return;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
            healthText.text = $"{currentHealth}/{maxHealth}";
            physicalArmorSlider.maxValue = maxPhysicalArmor;
            physicalArmorSlider.value = currentPhysicalArmor;
            physicalArmorText.text = $"{currentPhysicalArmor}/{maxPhysicalArmor}";
            magicArmorSlider.maxValue = maxMagicArmor;
            magicArmorSlider.value = currentMagicArmor;
            magicArmorText.text = $"{currentMagicArmor}/{maxMagicArmor}";
        }

        public void TakeDamage(Weapon.DamageType type, int damage) // todo: add type of damage
        {
            if (!isAlive) return;
            var actualDamage = Random.Range(damage - (int) (damage * 0.9f), damage + (int) (damage * 0.1f));
            if (currentPhysicalArmor > 0)
            {
                if (currentPhysicalArmor - actualDamage < 0)
                {
                    currentHealth -= actualDamage - currentPhysicalArmor;
                    currentPhysicalArmor = 0;
                }
                else
                {
                    currentPhysicalArmor -= actualDamage;
                }
            }
            else
            {
                currentHealth -= actualDamage;
            }

            if (currentHealth > 0) // normal hurt sfx
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.volume = 1;
                audioSource.PlayOneShot(hurtAudioClips[Random.Range(0, hurtAudioClips.Length)]);
                Damaged?.Invoke();
            }

            if (currentHealth <= 0) // death sfx
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.volume = 0.7f;
                audioSource.PlayOneShot(deadAudioClips[Random.Range(0, deadAudioClips.Length)]);
                healthSlider.value = 0;
                healthText.text = $"{0}/{maxHealth}";
                isAlive = false;
                // ragdoll death
            }

            var newHitPopup = Instantiate(hitPopupPrefab, hitPopupsParent.transform, false);
            newHitPopup.GetComponent<TextMeshProUGUI>().text = actualDamage.ToString();
            activeHitPopups.Add(newHitPopup);
        }

        private void OnEnable()
        {
            this.Damaged += DamageEventExample;
        }

        private void OnDisable()
        {
            this.Damaged -= DamageEventExample;
        }

        public void DamageEventExample()
        {
            // Debug.Log("Enemy damaged event example");
        }
    }
}