using HighlightPlus;
using UnityEngine;

namespace intheclouds
{
    public class HighlightProfileManager : MonoBehaviour
    {
        public HighlightProfile SocketHoverProfile;
        public HighlightProfile WeaponHandHoverProfile;
        public HighlightProfile HealingItemHandHoverProfile;
        public HighlightProfile MagicItemHandHoverProfile;
        public HighlightProfile PoiseItemHandHoverProfile;
        public HighlightProfile PropHandHoverProfile;

        public static HighlightProfileManager Instance;

        private void Awake()
        {
            Instance = this;
        }
    }
}
