using Eflatun.SceneReference;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace intheclouds
{
    public class LevelSelect : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField]
        private string sceneToLoad;


        public void Button_SelectScene(string sceneName)
        {
            sceneToLoad = sceneName;
        }
        
        public void Button_LoadScene()
        {
            SceneLoader.instance.GoToSceneAsync(sceneToLoad);
        }
    }
}