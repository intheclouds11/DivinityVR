using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace intheclouds
{
    public class RainSurfaceMaker : MonoBehaviour
    {
        private void OnParticleCollision(GameObject other)
        {
            // todo: check if other layer == ground. instantiate puddles. when certain # of puddles spawned, disable script 
        }
    }
}
