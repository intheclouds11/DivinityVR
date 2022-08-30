using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace intheclouds
{
    public class LookAt : MonoBehaviour
    {
        private GameObject target;
        private GameObject hitBy;

        private void Update()
        {
            if (!hitBy)
            {
                hitBy = transform.parent.parent.parent.parent.GetComponent<EnemyStats>().playerHitBy.LocalUserObjects.HVRPlayerController.gameObject;
            }

            target = hitBy;
            transform.LookAt(target.transform);
        }
    }
}