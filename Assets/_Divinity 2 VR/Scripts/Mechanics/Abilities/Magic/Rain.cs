using HurricaneVR.Framework.Core.Utils;
using UnityEngine;

namespace intheclouds
{
    public class Rain : AbilityBase
    {
        public Rigidbody rb;
        public AudioClip activatedClip;
        public RainSurfaceMaker rainSurfaceMaker;

        private void Update()
        {
            if (rb.velocity.y < -2f)
            {
                Activate();
            }
        }

        public void Activate()
        {
            rainSurfaceMaker.enabled = true;
            activatedVFX.transform.eulerAngles = new Vector3(-90, 0, 0);
            SFXPlayer.Instance.PlaySFX(activatedClip, transform.position);
            OnAbilityUsed();
        }
    }
}