using System;
using UnityEditor.Callbacks;
using UnityEngine;

namespace intheclouds
{
    public class ScriptsReloaded : MonoBehaviour
    {
        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            Debug.Log($"Script Compilation Complete");
        }
    }
}
