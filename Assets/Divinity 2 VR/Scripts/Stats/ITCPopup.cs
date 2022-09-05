using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ITCPopup : MonoBehaviour
{
    public bool staticPopup;
    public bool movingPopup;
    public bool growingPopup;
    public float translateSpeed;
    public float secondsToDestroy = 2;

    private void Awake()
    {
        Destroy(gameObject, secondsToDestroy);
    }

    private void Update()
    {
        if (!staticPopup)
        {
            if (movingPopup)
            {
                transform.Translate(0, translateSpeed * Time.deltaTime, 0);
            }

            if (growingPopup)
            {
                transform.localScale += (transform.localScale + new Vector3(0.001f, 0.001f, 0.001f)) * Time.deltaTime;
            }
        }
    }
}