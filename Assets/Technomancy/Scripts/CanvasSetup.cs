using HurricaneVR.Framework.Core.UI;
using UnityEngine;

namespace intheclouds
{
    public class CanvasSetup : MonoBehaviour
    {
        void Start()
        {
            // Add canvas to HVRInputModule
            FindObjectOfType<HVRInputModule>().AddCanvas(GetComponent<Canvas>());
        }
    }
}
