using System;
using System.Collections;
using System.Collections.Generic;
using HurricaneVR.Framework.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace intheclouds
{
    public class MainMenu : MonoBehaviour
    {
        public void Button_NewGame()
        {
            SceneLoader.Instance.GoToSceneAsync(1);
        }

        public void Button_LoadGame()
        {
            // load each save file json and show in a list with Scene name, time, and date (HH:MM:SS MM/DD/YYYY)
        }
        
        public void Button_Setting()
        {
            // load settings json
        }
        
        public void ExitLeverTest(int i)
        {
            if (i == 2)
            {
                Application.Quit();
            }
        }
    }
}
