using HurricaneVR.Framework.Core.Utils;
using UnityEngine;

namespace intheclouds
{
    public class TreasureChest : MonoBehaviour
    {
        public bool opened;
        public int gold = 12;
        public AudioClip openSFX;
        public AudioClip closeSFX;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Hand"))
            {
                if (!opened)
                {
                    opened = true;
                    SFXPlayer.Instance.PlaySFXAttach(openSFX, transform, 1, 1);
                    collision.gameObject.transform.root.GetComponent<PlayerStats>().Gold += gold;
                }
            }
        }
    }
}