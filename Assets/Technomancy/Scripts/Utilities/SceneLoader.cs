using System.Collections;
using HurricaneVR.Framework.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace intheclouds
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader instance;
        private bool _fadeoutEnded;

        private void Awake()
        {
            instance = this;
        }

        private void OnEnable()
        {
            HVRManager.Instance.ScreenFader.FadeEnd.AddListener(FadeEnded); 
        }

        private void OnDisable()
        {
            HVRManager.Instance.ScreenFader.FadeEnd.RemoveListener(FadeEnded); 
        }

        public void GoToSceneAsync(int sceneIndex)
        {
            StartCoroutine(GoToSceneAsyncRoutine(sceneIndex));
        }

        private IEnumerator GoToSceneAsyncRoutine(int sceneIndex)
        {
            HVRManager.Instance.ScreenFader.Fade(1, 1);
            
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
            operation.allowSceneActivation = false;

            while (!_fadeoutEnded && !operation.isDone)
            {
                yield return null;
            }
            
            operation.allowSceneActivation = true;
            _fadeoutEnded = false;
            
            HVRManager.Instance.ScreenFader.Fade(0, 0.5f);
        }

        private void FadeEnded()
        {
            _fadeoutEnded = true;
        }
    }
}
