using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using SolClovser.VRDebugGizmos;
using UnityEngine;

namespace intheclouds
{
    public class ITCHandGrabber : HVRHandGrabber
    {
        public ITCPopup HoverInfo;

        protected override void OnReleased(HVRGrabbable grabbable)
        {
            base.OnReleased(grabbable);

            // if (grabbable.hasImpactHandler)
            // {
            //     // Get pointing direction. Add force to grabbable Rb until grabbable collides with something
            //     var itcGrabbable = grabbable as ITCGrabbable;
            //     if (itcGrabbable)
            //     {
            //         itcGrabbable.FollowHandDirectionAfterThrown(RaycastOrigin);
            //     }
            // }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            // var pointingDir = RaycastOrigin.forward;
            // var raycastPos = RaycastOrigin.position;
            // VRDebugGizmos.DrawLine(this, "fwd", raycastPos, raycastPos + pointingDir, 0.01f, Color.blue);
            // VRDebugGizmos.DrawLine(this, "up", raycastPos, raycastPos + RaycastOrigin.up, 0.1f, Color.yellow);
            // VRDebugGizmos.DrawLine(this, "right", raycastPos, raycastPos + RaycastOrigin.right, 0.1f, Color.red);
        }
    }
}
