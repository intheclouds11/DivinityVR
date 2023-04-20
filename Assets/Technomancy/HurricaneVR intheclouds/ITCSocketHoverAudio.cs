using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.Sockets;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;

namespace intheclouds
{
    public class ITCSocketHoverAudio : HVRSocketHoverAction
    {
        public AudioClip ValidHoverClip;
        public AudioClip ValidHoverExitClip;
        public AudioClip InvalidHoverClip;

        public override void OnHoverEnter(HVRSocket socket, HVRGrabbable grabbable, bool isValid)
        {
            if (isValid && ValidHoverClip)
            {
                SFXPlayer.Instance.PlaySFX(ValidHoverClip, transform.position);
            }
            else if (!isValid && InvalidHoverClip)
            {
                SFXPlayer.Instance.PlaySFX(InvalidHoverClip, transform.position);
            }
        }

        public override void OnHoverExit(HVRSocket socket, HVRGrabbable grabbable, bool isValid)
        {
            if (isValid && ValidHoverExitClip)
            {
                SFXPlayer.Instance.PlaySFX(ValidHoverExitClip, transform.position);
            }
        }
    }
}