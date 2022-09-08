using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetITC : MonoBehaviour
{
    public GameObject target;
    public float smoothFactor;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position =Vector3.Lerp(transform.position, target.transform.position, smoothFactor * Time.deltaTime);
        // transform.rotation =Quaternion.Lerp(transform.rotation, target.transform.rotation, smoothFactor * Time.deltaTime);
    }
}
