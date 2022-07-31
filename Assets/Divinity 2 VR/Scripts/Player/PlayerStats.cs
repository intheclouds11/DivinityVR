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
        private float currentStamina = 0;
        public float maxStamina;
        [SerializeField] private Slider staminaSlider;
        public float staminaRecoveryRate;
        public float staminaDepletionRateSprinting;
        private HVRPlayerController playerController;

        public event Action Damaged; // use for other classes to know when player is damaged

        private void Start()
        {
            playerController = GetComponent<HVRPlayerController>();
            maxHealth = playerStatsSO.health;
            currentHealth = maxHealth;
            maxStamina = playerStatsSO.stamina;
            currentStamina = maxStamina;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
            staminaRecoveryRate = playerStatsSO.staminaRecoveryRate;
            staminaDepletionRateSprinting = playerStatsSO.staminaDepletionRateSprinting;
        }

        private void Update()
        {
            staminaSlider.value = currentStamina;
            if (playerController.Sprinting)
            {
                currentStamina -= Time.deltaTime * staminaDepletionRateSprinting;
                if (currentStamina <= 0)
                {
                    playerController.Sprinting = false;
                    playerController.CanSprint = false;
                }
            }
            else
            {
                if (currentStamina <= maxStamina)
                {
                    currentStamina += Time.deltaTime * staminaRecoveryRate;
                }

                if (currentStamina >= maxStamina)
                {
                    playerController.CanSprint = true;
                }
            }
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