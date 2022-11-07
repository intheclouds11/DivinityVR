using UnityEngine;

namespace intheclouds
{
    public class FadeObjectOnLoad : MonoBehaviour
    {
        public MeshRenderer meshRenderer;
        public float fadeSpeed;
        public bool fadeIn, fadeOut;
        public float destroyTimer;
        public float timeToDestroy = 30f;
        public bool useDestroyTimer;

        private void Start()
        {
            if (fadeIn)
            {
                meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.b, meshRenderer.material.color.g, 0);
            }
        }

        void Update()
        {
            if (useDestroyTimer)
            {
                destroyTimer += Time.deltaTime;
                if (destroyTimer >= timeToDestroy)
                {
                    Destroy(this.gameObject);
                }
            }

            if (fadeIn)
            {
                FadeIn();
            }

            if (fadeOut)
            {
                FadeOut();
            }
        }

        private void FadeIn()
        {
            Color objectColor = meshRenderer.material.color;
            float fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);
            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            meshRenderer.material.color = objectColor;

            if (objectColor.a >= 1)
            {
                fadeIn = false;
            }
        }

        private void FadeOut()
        {
            Color objectColor = meshRenderer.material.color;
            float fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);
            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            meshRenderer.material.color = objectColor;

            if (objectColor.a <= 0)
            {
                fadeOut = false;
            }
        }
    }
}