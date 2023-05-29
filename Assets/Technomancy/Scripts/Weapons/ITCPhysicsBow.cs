using HurricaneVR.Framework.Weapons.Bow;

namespace intheclouds
{
    public class ITCPhysicsBow : HVRPhysicsBow
    {
        private PlayerStats _wieldingUser;
        public int requiredAP;

        protected override void UpdateBow()
        {
            if (_wieldingUser == null) return;

            if (_wieldingUser.CanPerformActions() && _wieldingUser.CurrentAP >= requiredAP)
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
            arrow.GetComponent<ITCArrow>().player = _wieldingUser;
        }

        protected override void OnArrowShot()
        {
            if (_wieldingUser.InCombat)
            {
                _wieldingUser.UseAP(requiredAP);
            }

            base.OnArrowShot();
        }

        public void UpdateWielder()
        {
            _wieldingUser = Grabbable.PrimaryGrabber.transform.GetComponentInParent<LocalUserObjects>().PlayerStats;
        }
    }
}