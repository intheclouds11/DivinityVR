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
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider apSlider;
        public int currentHealth;
        public int maxHealth;
        public int currentAP;
        public int maxAP;

        public event Action Damaged; // use for other classes to know when player is damaged

        private void Start()
        {
            maxHealth = playerStatsSO.maxHealth;
            currentHealth = maxHealth;
            maxAP = playerStatsSO.maxAP;
            currentAP = maxAP;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
            apSlider.maxValue = maxAP;
            apSlider.value = currentAP;
        }
        
        public void TakeDamage(float damage)
        {
            Damaged?.Invoke();
            // currentHealth -= damage;
            healthSlider.value = currentHealth;
            if (currentHealth <= 0)
            {
                GameManager.Instance.UpdateGameState(GameState.Lose);
            }
        }
    }
}