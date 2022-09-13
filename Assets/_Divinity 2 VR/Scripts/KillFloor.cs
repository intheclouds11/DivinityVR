using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace intheclouds
{
    public class KillFloor : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Grabbable") || other.gameObject.layer == LayerMask.NameToLayer("HitEnemy"))
            {
                if (other.transform.parent != null)
                {
                    Destroy(other.transform.parent.gameObject);
                }
                else
                {
                    Destroy(other.gameObject);
                }
            }
        }
    }
}