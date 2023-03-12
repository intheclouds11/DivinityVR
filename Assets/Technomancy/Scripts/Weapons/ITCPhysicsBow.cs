using HurricaneVR.Framework.Weapons.Bow;

namespace intheclouds
{
    public class ITCPhysicsBow : HVRPhysicsBow
    {
        private PlayerStats wieldingUser;
        public int requiredAP;

        protected override void UpdateBow()
        {
            if (wieldingUser == null) return;

            if ((wieldingUser.Turn || !wieldingUser.InCombat) && wieldingUser.CurrentAP >= requiredAP && !wieldingUser.LocalUserObjects.spiritWander.isActivated)
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
            arrow.GetComponent<ITCArrow>().player = wieldingUser;
        }

        protected override void OnArrowShot()
        {
            if (wieldingUser.InCombat)
            {
                wieldingUser.UseAP(requiredAP);
            }

            base.OnArrowShot();
        }

        public void UpdateWielder()
        {
            wieldingUser = Grabbable.PrimaryGrabber.transform.GetComponentInParent<LocalUserObjects>().PlayerStats;
        }
    }
}