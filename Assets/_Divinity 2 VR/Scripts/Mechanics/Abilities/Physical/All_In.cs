using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace intheclouds
{
    public class All_In : AbilityBase
    {
        // Get currently held weapon, change hold type of grabbable to manual (code to let go), multiply weapon damage by 1.25, after deal damage divide weapon damage by 1.25

        protected override void OnEnable()
        {
            // castingHand.GrabbedTarget.GetComponent<>()
            base.OnEnable();
        }
    }
}
