using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace intheclouds
{
    public class LookAt : MonoBehaviour
    {
        public Transform target;

        private void Awake()
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