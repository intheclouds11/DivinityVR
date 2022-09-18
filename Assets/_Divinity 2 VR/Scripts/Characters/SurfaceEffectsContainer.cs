using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace intheclouds
{
    public class SurfaceEffectsContainer : MonoBehaviour
    {
        public static SurfaceEffectsContainer Instance;
        public List<SurfaceEffect> surfaceEffectsList = new List<SurfaceEffect>();

        void Start()
        {
            Instance = this;
        }

        void Update()
        {
        
        }
    }
}
