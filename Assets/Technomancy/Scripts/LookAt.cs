using HurricaneVR.Framework.Core;
using NaughtyAttributes;
using UnityEngine;

namespace intheclouds
{
    public class LookAt : MonoBehaviour
    {
        public Transform target;

        private void Start()
        {
            if (!target)
            {
                target = HVRManager.Instance.Camera.transform;
            }
        }

        private void Update()
        {
            if (target)
            {
                transform.rotation = Quaternion.LookRotation(transform.position - target.position);
            }
        }
    }
}