using UnityEngine;

namespace intheclouds
{
    public class Rain : AbilityBase
    {
        public Rigidbody rb;
        
        private void Update()
        {
            if (rb.velocity.y < -2f && AbilitySpawnLocator.instance.IsSelectionValid)
            {
                Activate(AbilitySpawnLocator.instance.SelectionLocation, new Vector3(-90, 0, 0));
            }
        }
        
        private void Activate(Vector3 position, Vector3 rotation)
        {
            if (activatedVFX != null)
            {
                activatedVFX.transform.parent = null;
                activatedVFX.transform.position = position;
                activatedVFX.transform.eulerAngles = rotation;
                activatedVFX.SetActive(true);
            }

            if (casterVFX != null)
            {
                casterVFX.SetActive(true);
            }

            OnAbilityUsed();
            ResetAbilityTransform();
        }
    }
}