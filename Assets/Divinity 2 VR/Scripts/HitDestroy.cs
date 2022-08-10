using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HitDestroy : MonoBehaviour
{
    public float destroyVelocity;
    public GameObject swapToGameObject;

    private void OnCollisionEnter(Collision collision)
    {
        if (ReachedActionVelocity(collision))
        {
            swapToGameObject.SetActive(true);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("hit a destroyable, not enough velocity to destroy");
        }
    }

    public bool ReachedActionVelocity(Collision collision)
    {
        return collision.relativeVelocity.magnitude > destroyVelocity;
    }
}