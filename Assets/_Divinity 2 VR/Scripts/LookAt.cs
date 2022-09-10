using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace intheclouds
{
    public class LookAt : MonoBehaviour
    {
        private Transform target;

        private void Awake()
        {
            target = GameManager.Instance.FindControlledPlayer().LocalUserObjects.Camera.transform;
        }

        private void Update()
        {
            if (target != null)
            {
                transform.LookAt(target.transform);
            }
        }
    }
}