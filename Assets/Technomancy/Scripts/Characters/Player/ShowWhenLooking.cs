using UnityEngine;

namespace intheclouds
{
    public class ShowWhenLooking : MonoBehaviour
    {
        public GameObject objectToShowAndHide;
        public LayerMask LayerMask;
        private Ray _ray;

        private void Start()
        {
            objectToShowAndHide.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, 5, LayerMask, QueryTriggerInteraction.Collide))
            {
                if (hitInfo.collider.gameObject.CompareTag("MakeVisibleWhenLookingAt"))
                {
                    objectToShowAndHide.SetActive(true);
                }
                else
                {
                    objectToShowAndHide.SetActive(false);
                }
            }
        }
    }
}