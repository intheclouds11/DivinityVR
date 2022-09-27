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
        public int requiredAP;

        protected override void UpdateBow()
        {
            if (wieldingUser == null) return;

            if (wieldingUser.Turn && wieldingUser.CurrentAP >= requiredAP && !wieldingUser.LocalUserObjects.spiritWander.isActivated)
            {
                NockGrabbable.enabled = true;
            }
            else if (!wieldingUser.ExplorationMode || wieldingUser.LocalUserObjects.spiritWander.isActivated)
            {
                NockGrabbable.enabled = false;
            }
        }

        protected override void OnArrowNocked(HVRArrow arrow)
        {
            base.OnArrowNocked(arrow);
            arrow.GetComponent<ITCArrow>().player = Grabbable.PrimaryGrabber.transform.root.GetComponentInChildren<PlayerStats>();
        }

        public void UpdateWielder()
        {
            wieldingUser = Grabbable.PrimaryGrabber.transform.root.GetComponent<LocalUserObjects>().PlayerStats;
        }
    }
}