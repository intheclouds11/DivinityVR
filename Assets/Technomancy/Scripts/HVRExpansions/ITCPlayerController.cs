using System;
using HurricaneVR.Framework.Core.Player;
using HurricaneVR.Framework.Core.Utils;
using Sigtrap.VrTunnellingPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace intheclouds
{
    public class ITCPlayerController : HVRPlayerController
    {
        public TunnellingMobile TunnellingMobile;

        protected override void Awake()
        {
            TunnellingMobile = Camera.GetComponent<TunnellingMobile>();
            if (TunnelMovementInput)
            {
                TunnellingMobile.useVelocity = true;
            }
            base.Awake();
        }

        protected override void HandleSmoothRotation()
        {
            var input = GetTurnAxis().x;
            float newInput = GetSmoothedTurnAxis().x;
            float threshold = SmoothInputForSmoothTurn ? SmoothTurnThresholdSmoothed : SmoothTurnThreshold;

            if (Math.Abs(input) < threshold || teleportCooldown > 0)
            {
                if (TunnelTurningInput && TunnellingMobile.GetAngularVelocitySmoothed() <= 0.15f)
                {
                    TunnellingMobile.useAngularVelocity = false;
                }
                return;
            }

            if (TunnelTurningInput)
            {
                if (!Teleporter.IsAiming)
                {
                    TunnellingMobile.useAngularVelocity = true;
                }
            }

            if (!SmoothInputForSmoothTurn)
            {
                if (input > 0)
                {
                    newInput = input.Remap(SmoothTurnThreshold, 1, 0, 1);
                }
                else
                {
                    newInput = input.Remap(-1, -SmoothTurnThreshold, -1, 0);
                }
            }

            var rotation = newInput * SmoothTurnSpeed * Time.deltaTime;
            var rotationVector = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + rotation, transform.eulerAngles.z);
            transform.rotation = Quaternion.Euler(rotationVector);
        }
    }
}
