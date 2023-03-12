using UnityEngine;

namespace intheclouds
{
    public class LookAt : MonoBehaviour
    {
        public Transform target;

        private void Start()
        {
            if (target == null)
            {
                target = GameManager.Instance.FindControlledPlayer().LocalUserObjects.Camera.transform;
            }
        }

        private void Update()
        {
            if (target != null)
            {
                transform.rotation = Quaternion.LookRotation(transform.position - target.position);
                // transform.LookAt(target.transform);
            }
        }
    }
}