using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace intheclouds
{
    public class LookAt : MonoBehaviour
    {
        private GameObject target;

        private void Update()
        {
            if (target)
            {
                transform.LookAt(target.transform);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                target = other.gameObject;
            }
        }
    }
}