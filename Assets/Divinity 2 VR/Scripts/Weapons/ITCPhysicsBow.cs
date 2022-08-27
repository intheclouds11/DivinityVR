using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Weapons.Bow;
using UnityEngine;

namespace intheclouds
{
    public class ITCPhysicsBow : HVRPhysicsBow
    {
        public int requiredAP = 2;
        private PlayerStats wieldingUser;

        protected override void UpdateBow()
        {
            if (wieldingUser == null) return;

            if (wieldingUser.explorationMode || (wieldingUser.turn && wieldingUser.currentAP >= requiredAP))
            {
                NockGrabbable.enabled = true;
            }
            else
            {
                NockGrabbable.enabled = false;
            }
        }

        protected override void OnArrowNocked(HVRArrow arrow)
        {
            base.OnArrowNocked(arrow);
            arrow.GetComponent<ITCArrow>().wieldingUser = Grabbable.PrimaryGrabber.transform.root.GetComponentInChildren<PlayerStats>();
        }

        protected override void OnArrowShot()
        {
            if (!wieldingUser.turn && !wieldingUser.explorationMode) return;

            if (!wieldingUser.explorationMode)
            {
                wieldingUser.UseAP(requiredAP);
            }
            else
            {
                if (!ITCPlayerInputs.Instance.debugInteractions)
                {
                    wieldingUser.explorationMode = false;
                }
            }

            base.OnArrowShot();
        }

        public void UpdateWielder()
        {
            if (Grabbable.PrimaryGrabber == null)
            {
                wieldingUser = null;
                Debug.Log("Weapon dropped! wieldingUser == null");
            }
            else
            {
                wieldingUser = Grabbable.PrimaryGrabber.transform.root.GetComponentInChildren<PlayerStats>();
                Debug.Log($"Weapon grabbed! wieldingUser: {wieldingUser.userName}");
            }
        }
    }
}