using HurricaneVR.Framework.Core.Utils;
using UnityEngine;

namespace intheclouds
{
    public class StatusEffect : MonoBehaviour
    {
        public bool ProcessOnEnabled;
        public int ChanceToApply = 100;
        public StatusEffectType type;
        public int effectAmount;
        public int cooldown;
        public int cooldownTimer;
        public AudioClip activatedClip;
        public GameObject activeVFX;
        public GameObject activatedVFX;
        public BaseStats CombatantWhoApplied;
        private BaseStats _combatant;
        private Transform _originalParent;
        private GameObject _spawnedActiveVFX;
        private GameObject _spawnedActivatedVFX;

        private void OnDestroy()
        {
            if (type == StatusEffectType.Stunned)
            {
                Debug.Log("Stun removed!");
                _combatant.Stun(false);
            }

            if (_spawnedActiveVFX)
            {
                var particles = _spawnedActiveVFX.GetComponent<ParticleSystem>();
                Destroy(_spawnedActiveVFX, particles ? particles.main.duration - particles.time : 2f);
            }

            if (_spawnedActivatedVFX)
            {
                var particles = _spawnedActivatedVFX.GetComponent<ParticleSystem>();
                Destroy(_spawnedActivatedVFX, particles ? particles.main.duration - particles.time : 2f);
            }
        }

        public void StatusEffectConstructor(StatusEffect effect, bool preExisting = false)
        {
            if (!_combatant)
            {
                _combatant = GetComponentInParent<BaseStats>();
            }

            type = effect.type;
            effectAmount = effect.effectAmount;
            cooldown = effect.cooldown;
            cooldownTimer = effect.cooldown;
            activatedClip = effect.activatedClip;
            ProcessOnEnabled = effect.ProcessOnEnabled;
            activeVFX = effect.activeVFX;
            activatedVFX = effect.activatedVFX;

            if (ProcessOnEnabled)
            {
                ActivateStatusEffect(preExisting);
            }
        }

        public void ActivateStatusEffect(bool preExisting = false)
        {
            int damage = (int) (effectAmount * (1 + _combatant.level * 0.5f));
            
            if (type == StatusEffectType.Burning)
            {
                _combatant.TakeDamage(null, damage, DamageType.Magic, ScalingType.Pyrokinetic, null);
            }
            else if (type == StatusEffectType.Regenerating)
            {
                _combatant.Heal(effectAmount);
            }
            else if (type == StatusEffectType.Stunned)
            {
                _combatant.Stun(true);
            }
            else if (type == StatusEffectType.Bleeding)
            {
                _combatant.TakeDamage(null, damage, DamageType.Magic, ScalingType.None, null);
            }
            else
            {
                Debug.LogWarning("No status effect type assigned!");
            }

            if (activatedClip)
            {
                SFXPlayer.Instance.PlaySFX(activatedClip, _combatant.attachToCombatantTransform.position, 1, 1);
            }

            if (!preExisting)
            {
                if (activeVFX && !_spawnedActiveVFX)
                {
                    _spawnedActiveVFX = Instantiate(activeVFX, _combatant.attachToCombatantTransform.position, Quaternion.identity, _combatant.attachToCombatantTransform);
                }

                if (activatedVFX)
                {
                    if (!_spawnedActivatedVFX)
                    {
                        _spawnedActivatedVFX = Instantiate(activatedVFX, _combatant.attachToCombatantTransform.position, Quaternion.identity,
                            _combatant.attachToCombatantTransform);
                    }
                    else
                    {
                        _spawnedActivatedVFX.SetActive(false);
                        _spawnedActivatedVFX.SetActive(true);
                    }
                }
            }
        }
    }
}