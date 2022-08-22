using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Weapons.Bow;
using UnityEngine;

namespace intheclouds
{
    public class Bow : MonoBehaviour
    {
        public int requiredHitAP = 2;
        private PlayerStats wieldingUser;
        private HVRGrabbable grabbable; 

        private void Awake()
        {
            grabbable = GetComponent<HVRGrabbable>();
        }

        private void Update()
        {
            if (wieldingUser != null && wieldingUser.playerTurnCombat && wieldingUser.currentAP >= requiredHitAP)
            {
                GetComponent<HVRArrowLoader>().enabled = true;
            }
            else
            {
                GetComponent<HVRArrowLoader>().enabled = false;
            }
        }

        public void UpdateWielder()
        {
            if (grabbable.PrimaryGrabber == null)
            {
                wieldingUser = null;
                Debug.Log("Weapon dropped! wieldingUser == null");
            }
            else
            {
                wieldingUser = grabbable.PrimaryGrabber.transform.root.GetComponentInChildren<PlayerStats>();
                Debug.Log($"Weapon grabbed! wieldingUser: {wieldingUser.userName}");
            }
        }
    }
}