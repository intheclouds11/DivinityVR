using HurricaneVR.Framework.Core.Utils;
using UnityEngine;

namespace intheclouds
{
    public class MainMenu : MonoBehaviour
    {
        public AudioClip newGameButtonClip;
        public AudioClip loadGameButtonClip;
        public AudioClip settingsButtonClip;
        
        public void Button_NewGame()
        {
            SceneLoader.instance.GoToSceneAsync(1);
            SFXPlayer.Instance.PlaySFXAttach(newGameButtonClip, SFXPlayer.Instance.transform, 1, 1f);
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
