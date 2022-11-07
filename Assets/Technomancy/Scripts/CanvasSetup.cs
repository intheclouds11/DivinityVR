using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.Core.UI;
using UnityEngine;

namespace intheclouds
{
    public class CanvasSetup : MonoBehaviour
    {
        void Start()
        {
            // Add canvas to HVRInputModule
            Debug.Log(GetComponent<Canvas>());
            Debug.Log(FindObjectOfType<LocalUserObjects>());
            Debug.Log(FindObjectOfType<LocalUserObjects>().userMenu);
            // FindObjectOfType<LocalUserObjects>().userMenu.GetComponentInChildren<HVRInputModule>().AddCanvas(GetComponent<Canvas>());
        }
    }
}
