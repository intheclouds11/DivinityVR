using HurricaneVR.Framework.Weapons;
using UnityEngine;
using UnityEngine.Serialization;

namespace HurricaneVR.Framework.Components
{
    public class HVRDamageHandler : HVRDamageHandlerBase
    {
        public float Life = 100f;

        public bool Damageable = true;

        public Rigidbody Rigidbody { get; private set; }

        [FormerlySerializedAs("Desctructible")]
        public HVRDestructible Destructible;

        void Start()
        {
            Rigidbody = GetComponent<Rigidbody>();
            if (!Destructible)
                Destructible = GetComponent<HVRDestructible>();
        }

        public override void TakeDamage(float damage)
        {
            if (Damageable)
            {
                Life -= damage;
            }

            if (Life <= 0)
            {
                if (Destructible)
                {
                    Destructible.Destroy();
                }
            }
        }

        public override void HandleDamageProvider(HVRDamageProvider damageProvider, Vector3 hitPoint, Vector3 direction)
        {
            base.HandleDamageProvider(damageProvider, hitPoint, direction);

            if (Rigidbody)
            {
                Rigidbody.AddForceAtPosition(direction.normalized * damageProvider.Force, hitPoint, ForceMode.Impulse);
            }
        }

    }
}