using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TresureChest : MonoBehaviour
{
    private AudioSource audioSource;
    public bool opened;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Hand"))
        {
            if (!opened)
            {
                opened = true;
                audioSource.Play();
            }
        }
    }
}
