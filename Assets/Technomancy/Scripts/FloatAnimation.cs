using System;
using UnityEngine;

public class FloatAnimation : MonoBehaviour
{
    public Vector3 lerpTo;
    public float speed;
    public float maxHeight = 1f;
    private float _initialLocalPosY;
    private bool _goUp = true;

    private void Start()
    {
        _initialLocalPosY = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (_goUp)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, transform.localPosition + lerpTo, speed * Time.deltaTime);
            if (Math.Abs(transform.localPosition.y - (_initialLocalPosY + maxHeight)) < 0.1)
            {
                _goUp = false;
            }
        }
        else
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, transform.localPosition - lerpTo, speed * Time.deltaTime);
            if (Math.Abs(transform.localPosition.y - (_initialLocalPosY - maxHeight)) < 0.1)
            {
                _goUp = true;
            }
        }
    }
}