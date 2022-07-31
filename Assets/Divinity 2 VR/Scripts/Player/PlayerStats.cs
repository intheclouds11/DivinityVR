using System;
using HurricaneVR.Framework.Core.Player;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace intheclouds
{
    public class PlayerStats : MonoBehaviour
    {
        public PlayerStatsSO playerStatsSO;
        public float currentHealth;
        public float maxHealth;
        [SerializeField] private Slider healthSlider;
        private float currentAP = 0;
        public float maxAP;
        [SerializeField] private Slider apSlider;

        public event Action Damaged; // use for other classes to know when player is damaged

        private void Start()
        {
            maxHealth = playerStatsSO.health;
            currentHealth = maxHealth;
            maxAP = playerStatsSO.actionPoints;
            currentAP = maxAP;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
            apSlider.maxValue = maxAP;
            apSlider.value = currentAP;
        }
        
        public void TakeDamage(float damage)
        {
            Damaged?.Invoke();
            currentHealth -= damage;
            healthSlider.value = currentHealth;
            if (currentHealth <= 0)
            {
                GameManager.Instance.UpdateGameState(GameState.Lose);
            }
        }
    }
}