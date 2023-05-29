using HighlightPlus;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;

namespace intheclouds
{
    public class ITCGrabbable : HVRGrabbable
    {
        public HighlightEffect highlightEffect { get; protected set; }
        public HighlightProfile originalHighlightProfile;
        public bool wasHighlighted;


        protected override void Awake()
        {
            base.Awake();
            highlightEffect = GetComponent<HighlightEffect>();
        }

        protected override void OnHoverEnter(HVRGrabberBase grabber)
        {
            base.OnHoverEnter(grabber);

            if (!hoveringGrabber)
            {
                hoveringGrabber = grabber;
                if (grabber is ITCForceGrabber or HVRHandGrabber)
                {
                    OnHandHoverEnter(grabber, this);
                }
                else
                {
                    var socket = grabber as ITCSocket;
                    if (socket && socket.IsValid(this))
                    {
                        OnSocketHoverEnter();
                    }
                }
            }
            else if (grabber is ITCForceGrabber or HVRHandGrabber)
            {
                secondHoveringHand = grabber;
            }
        }

        protected override void OnHoverExit(HVRGrabberBase grabber)
        {
            base.OnHoverExit(grabber);

            if (hoveringGrabber == grabber)
            {
                if (grabber is ITCForceGrabber or HVRHandGrabber)
                {
                    if (secondHoveringHand)
                    {
                        hoveringGrabber = secondHoveringHand;
                        secondHoveringHand = null;
                        return;
                    }

                    OnHandHoverExit(grabber, this);
                }
                else
                {
                    var socket = grabber as ITCSocket;
                    if (socket && socket.IsValid(this))
                    {
                        OnSocketHoverExit();
                    }
                }

                hoveringGrabber = null;
            }
            else if (secondHoveringHand)
            {
                secondHoveringHand = null;
            }
        }

        private void OnHandHoverEnter(HVRGrabberBase grabber, ITCGrabbable grabbable)
        {
            if (!IsHandGrabbed)
            {
                if (highlightEffect)
                {
                    wasHighlighted = highlightEffect.highlighted;
                    highlightEffect.highlighted = true;
                }

                var forceGrabber = grabber as ITCForceGrabber;
                if (forceGrabber)
                {
                    forceGrabber.HoverInfo.HandHovered(grabbable);
                }
                else
                {
                    var handGrabber = grabber as ITCHandGrabber;
                    if (handGrabber)
                    {
                        handGrabber.HoverInfo.HandHovered(grabbable);
                    }
                }
            }
        }

        private void OnHandHoverExit(HVRGrabberBase grabber, ITCGrabbable grabbable)
        {
            if (highlightEffect)
            {
                highlightEffect.highlighted = wasHighlighted;
            }

            var forceGrabber = grabber as ITCForceGrabber;
            if (forceGrabber)
            {
                forceGrabber.HoverInfo.HandUnhovered(grabbable);
            }
            else
            {
                var handGrabber = grabber as ITCHandGrabber;
                if (handGrabber)
                {
                    handGrabber.HoverInfo.HandUnhovered(grabbable);
                }
            }
        }

        private void OnSocketHoverEnter()
        {
            SocketHighlight();
        }

        private void OnSocketHoverExit()
        {
            UnSocketHighlight();
        }

        public void SocketHighlight()
        {
            if (highlightEffect)
            {
                originalHighlightProfile = highlightEffect.profile;
                wasHighlighted = highlightEffect.highlighted;
                highlightEffect.ProfileLoad(HighlightProfileManager.instance.SocketHoverProfile);
                highlightEffect.highlighted = true;
            }
        }

        public void UnSocketHighlight()
        {
            if (highlightEffect)
            {
                highlightEffect.highlighted = false;
                highlightEffect.ProfileLoad(originalHighlightProfile);
            }
        }
    }
}