using HighlightPlus;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;

namespace intheclouds
{
    public class ITCSocket : HVRSocket
    {
        private HighlightEffect highlightEffect;
        private HighlightProfile prevHighlightProfile;
        private bool wasHighlighted;
        
        
        protected override void OnHoverEnter(HVRGrabbable grabbable)
        {
            base.OnHoverEnter(grabbable);
            // highlightEffect = grabbable.GetComponent<HighlightEffect>();
            // if (highlightEffect)
            // {
            //     prevHighlightProfile = highlightEffect.profile;
            //     wasHighlighted = highlightEffect.highlighted;
            //     highlightEffect.ProfileLoad(HighlightProfileManager.Instance.SocketHoverProfile);
            //     highlightEffect.highlighted = true;
            // }
        }

        protected override void OnHoverExit(HVRGrabbable grabbable)
        {
            base.OnHoverExit(grabbable);
            // if (highlightEffect)
            // {
            //     highlightEffect.highlighted = wasHighlighted;
            //     highlightEffect.ProfileLoad(prevHighlightProfile);
            //     highlightEffect = null;
            // }
        }
    }
}
