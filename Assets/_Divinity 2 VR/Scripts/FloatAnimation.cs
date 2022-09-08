using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FloatAnimation : MonoBehaviour
{
    public Vector3 lerpTo;
    public float speed;
    public float maxHeight = 1f;
    private float initialLocalPosY;
    private bool goUp = true;

    private void Start()
    {
        initialLocalPosY = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (goUp)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, transform.localPosition + lerpTo, speed * Time.deltaTime);
            if (Math.Abs(transform.localPosition.y - (initialLocalPosY + maxHeight)) < 0.1)
            {
                goUp = false;
            }
        }
        else
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, transform.localPosition - lerpTo, speed * Time.deltaTime);
            if (Math.Abs(transform.localPosition.y - (initialLocalPosY - maxHeight)) < 0.1)
            {
                goUp = true;
            }
        }
    }
}