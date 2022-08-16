using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    public float secondsToDestroy = 2;

    private void Awake()
    {
        Destroy(gameObject, secondsToDestroy);
    }
}
