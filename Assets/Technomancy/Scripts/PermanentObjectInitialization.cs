using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace intheclouds
{
    public class PermanentObjectInitialization : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void PermObjectInitialization()
        {
            var startup = Instantiate(Resources.Load("StartupObjects")) as GameObject;
            DontDestroyOnLoad(startup);
        }
    }
}
