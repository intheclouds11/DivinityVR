using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace intheclouds
{
    public class ShowWhenLooking : MonoBehaviour
    {
        public GameObject objectToShowAndHide;
        private Ray ray;

        private void Start()
        {
            objectToShowAndHide.SetActive(false);
        }

        private void FixedUpdate()
        {
            ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
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