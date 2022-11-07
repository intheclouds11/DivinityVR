using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace intheclouds
{
    public class MainMenu : MonoBehaviour
    {
        public void Button_NewGame()
        {
            
        }

        public void Button_LoadGame()
        {
            // load each save file json and show in a list with Scene name, time, and date (HH:MM:SS MM/DD/YYYY)
        }
        
        public void Button_Setting()
        {
            // load settings json
        }
        
        public void Button_Exit()
        {
            Application.Quit();
        }

        public void ExitLeverTest()
        {
            Debug.Log("exit lever activated!");
        }
    }
}
