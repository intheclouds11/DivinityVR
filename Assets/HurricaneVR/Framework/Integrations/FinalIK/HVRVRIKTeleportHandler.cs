using RootMotion.FinalIK;
using UnityEngine;

namespace HurricaneVR.Framework.Integrations.FinalIK
{
    [RequireComponent(typeof(VRIK))]
    public class HVRVRIKTeleportHandler : MonoBehaviour
    {
        public VRIK VRIK;
    
        public void Awake()
        {
            VRIK = GetComponent<VRIK>();
        }

        public void AfterTeleport()
        {
            if (gameObject.activeInHierarchy)
            {
                VRIK.solver.Reset();
            }
        }
    }
}