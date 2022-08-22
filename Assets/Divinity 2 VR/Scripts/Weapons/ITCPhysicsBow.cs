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
            base.UpdateBow();
            if (wieldingUser == null) return;

            if (wieldingUser.explorationMode)
            {
                NockGrabbable.enabled = true;
            }
            else if (wieldingUser.playerTurnCombat && wieldingUser.currentAP >= requiredAP)
            {
                NockGrabbable.enabled = true;
            }
            else
            {
                NockGrabbable.enabled = false;
            }
        }

        protected override void OnArrowShot()
        {
            wieldingUser.UseAP(requiredAP);
            Arrow.gameObject.layer = LayerMask.NameToLayer("Grabbable");
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