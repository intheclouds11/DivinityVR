using HurricaneVR.Framework.Core.Grabbers;

namespace intheclouds
{
    public class ITCSocket : HVRSocket
    {
        public override void OnHandGrabberEntered(HVRGrabberBase grabber)
        {
            base.OnHandGrabberEntered(grabber);

            var itcGrabbable = GrabbedTarget as ITCGrabbable;
            if (itcGrabbable)
            {
                itcGrabbable.SocketHighlight();
            }
        }

        public override void OnHandGrabberExited(HVRGrabberBase grabber)
        {
            base.OnHandGrabberExited(grabber);
            
            var itcGrabbable = GrabbedTarget as ITCGrabbable;
            if (itcGrabbable)
            {
                itcGrabbable.UnSocketHighlight();
            }
        }
    }
}
