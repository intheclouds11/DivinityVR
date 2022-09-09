using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace intheclouds
{
    public class ToggleVisorVisibility : MonoBehaviour
    {
        public void Toggle()
        {
            if (transform.parent.name == "Visor Socket")
            {
                gameObject.layer = LayerMask.NameToLayer("InvisibleToMainCamera");
        
                foreach (Transform child in gameObject.transform)
                {
                    if (null == child)
                    {
                        continue;
                    }
        
                    child.gameObject.layer = LayerMask.NameToLayer("InvisibleToMainCamera");
                }
            }
            
            else
            {
                gameObject.layer = LayerMask.NameToLayer("Grabbable");
        
                foreach (Transform child in gameObject.transform)
                {
                    if (null == child)
                    {
                        continue;
                    }
        
                    child.gameObject.layer = LayerMask.NameToLayer("Grabbable");
                }
            }
        }
    }
}