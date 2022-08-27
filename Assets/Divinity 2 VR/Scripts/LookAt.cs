using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace intheclouds
{
    public class LookAt : MonoBehaviour
    {
        public GameObject target;

        void FixedUpdate()
        {
            transform.LookAt(target.transform);
        }
    }
}