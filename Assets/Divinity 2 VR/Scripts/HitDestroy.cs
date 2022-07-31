using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HitDestroy : MonoBehaviour
{
    [FormerlySerializedAs("destroySpeed")] public float destroyVelocity;
    public GameObject destroyedObj;

    private void OnCollisionEnter(Collision collision)
    {
        var collisionGameObject = collision.gameObject;
        if (collisionGameObject.CompareTag("Sword"))
        {
            Debug.Log(collision.relativeVelocity);
            Debug.Log(collision.relativeVelocity.x);
            Debug.Log(collision.relativeVelocity.y);
            Debug.Log(collision.relativeVelocity.z);
            if (ReachedActionVelocity(collision))
            {
                destroyedObj.SetActive(true);
                collisionGameObject.transform.position = this.transform.position;
                collisionGameObject.transform.rotation = this.transform.rotation;
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("hit/touch");
            }
        }
    }

    public bool ReachedActionVelocity(Collision collision)
    {
        return collision.relativeVelocity.x > destroyVelocity || collision.relativeVelocity.x < -destroyVelocity ||
               collision.relativeVelocity.y > destroyVelocity || collision.relativeVelocity.y < -destroyVelocity ||
               collision.relativeVelocity.z > destroyVelocity || collision.relativeVelocity.z < -destroyVelocity;
    }
}