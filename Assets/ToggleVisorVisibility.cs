using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace intheclouds
{
    public class ToggleVisorVisibility : MonoBehaviour
    {
        public void Toggle()
        {
            if (gameObject.layer == LayerMask.NameToLayer("Grabbable"))
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
            else if (gameObject.layer == LayerMask.NameToLayer("InvisibleToMainCamera"))
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

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}