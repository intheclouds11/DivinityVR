using HurricaneVR.Framework.Core.Utils;
using UnityEngine;

namespace intheclouds
{
    public class Rain : AbilityBase
    {
        public Rigidbody rb;
        public AudioClip activatedClip;

        private void Update()
        {
            if (rb.velocity.y < -2f)
            {
                Activate();
            }
        }

        public void Activate()
        {
            activatedVFX.transform.parent = null;
            activatedVFX.transform.eulerAngles = new Vector3(-90, 0, 0);
            activatedVFX.SetActive(true);
            SFXPlayer.Instance.PlaySFX(activatedClip, transform.position);
            OnMagicUsed();
            enabled = false;
            Destroy(gameObject);
        }
    }
}