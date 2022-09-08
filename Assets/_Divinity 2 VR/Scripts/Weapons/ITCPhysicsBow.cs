using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Weapons.Bow;
using UnityEngine;

namespace intheclouds
{
    public class ITCPhysicsBow : HVRPhysicsBow
    {
        private PlayerStats wieldingUser;

        protected override void UpdateBow()
        {
            if (wieldingUser == null) return;

            // if (wieldingUser.ExplorationMode || (wieldingUser.Turn && wieldingUser.CurrentAP >= requiredAP))
            // {
            //     NockGrabbable.enabled = true;
            // }
            // else
            // {
            //     NockGrabbable.enabled = false;
            // }
        }

        protected override void OnArrowNocked(HVRArrow arrow)
        {
            base.OnArrowNocked(arrow);
            arrow.GetComponent<ITCArrow>().wieldingUser = Grabbable.PrimaryGrabber.transform.root.GetComponentInChildren<PlayerStats>();
        }

        public void UpdateWielder()
        {
            wieldingUser = Grabbable.PrimaryGrabber.transform.root.GetComponent<LocalUserObjects>().PlayerStats;
        }
    }
}