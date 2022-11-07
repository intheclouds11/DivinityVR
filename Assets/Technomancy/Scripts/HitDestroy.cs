using UnityEngine;

namespace intheclouds
{
    public class HitDestroy : MonoBehaviour
    {
        public float destroyVelocity;
        public GameObject swapToGameObject;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.relativeVelocity.magnitude > destroyVelocity)
            {
                swapToGameObject.SetActive(true);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("hit a destroyable, not enough velocity to destroy");
            }
        }
    }
}