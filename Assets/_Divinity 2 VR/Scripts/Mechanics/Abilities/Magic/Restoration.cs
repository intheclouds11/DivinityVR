using System.Collections;
using HurricaneVR.Framework.Core.Utils;
using HurricaneVR.Framework.Shared;
using UnityEngine;

namespace intheclouds
{
    public class Restoration : AbilityBase
    {
        public Rigidbody rb;

        private void Update()
        {
            if (rb.velocity.y < -2f)
            {
                if (abilityPointer.combatantSelected != null)
                {
                    Activate(abilityPointer.combatantSelected);
                }
                else
                {
                    Activate(caster);
                }
            }
        }

        private void Activate(BaseStats combatant)
        {
            combatant.Heal(baseAmount); // add particle effect in Heal?
            combatant.statusEffectsContainer.TryAddStatusEffect(statusEffect);
            
            if (activatedVFX != null)
            {
                activatedVFX.transform.parent = null;
                activatedVFX.transform.position = combatant.transform.position;
                activatedVFX.transform.eulerAngles = combatant.transform.eulerAngles;
                activatedVFX.SetActive(true);
            }

            if (casterVFX != null)
            {
                StartCoroutine(ScaleParticles());
            }

            enabled = false;
        }

        private IEnumerator ScaleParticles()
        {
            while (casterVFX.transform.localScale.x <= 3)
            {
                casterVFX.transform.localScale *= 1 + Time.deltaTime;
                yield return null;
            }

            OnAbilityUsed();
            ResetAbilityTransform();
        }
    }
}