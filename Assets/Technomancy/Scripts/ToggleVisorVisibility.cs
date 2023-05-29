using UnityEngine;

namespace intheclouds
{
    public class ToggleVisorVisibility : MonoBehaviour
    {
        public void Toggle()
        {
            if (gameObject.layer != LayerMask.NameToLayer("InvisibleToMainCamera"))
            {
                gameObject.layer = LayerMask.NameToLayer("InvisibleToMainCamera");
        
                foreach (Transform child in gameObject.transform)
                {
                    if (!child)
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
                    if (!child)
                    {
                        continue;
                    }
        
                    child.gameObject.layer = LayerMask.NameToLayer("Grabbable");
                }
            }
        }
    }
}