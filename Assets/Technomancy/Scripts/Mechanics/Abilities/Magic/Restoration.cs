using System.Collections;
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
            combatant.statusEffectsContainer.TryAddStatusEffect(statusEffect);
            
            if (activatedVFX != null)
            {
                activatedVFX.transform.parent = combatant.transform.GetChild(0);
                activatedVFX.transform.localPosition = Vector3.zero;
                activatedVFX.transform.eulerAngles = Vector3.zero;
                activatedVFX.SetActive(true);
            }

            OnAbilityUsed();
            ResetAbilityTransform();

            enabled = false;
        }
    }
}