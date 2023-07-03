using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace intheclouds
{
    public class Orbit : MonoBehaviour
    {
        public float cycleLength = 5;
        void Start()
        {
            transform.DOMove(transform.position + transform.forward * 10, cycleLength).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }
    }
}
