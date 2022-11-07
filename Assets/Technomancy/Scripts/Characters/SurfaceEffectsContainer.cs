using System.Collections.Generic;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;

namespace intheclouds
{
    public class SurfaceEffectsContainer : MonoBehaviour
    {
        public static SurfaceEffectsContainer Instance;
        public List<SurfaceEffect> surfaceEffectsList = new List<SurfaceEffect>();
        private float cooldownTimerNoCombat;
        private BaseStats combatant;

        void Start()
        {
            Instance = this;
        }

        void Update()
        {
            if (GameManager.Instance.state == GameState.Exploration)
            {
                CooldownExploration();
            }
        }

        public void CooldownExploration()
        {
            if (cooldownTimerNoCombat < 2)
            {
                cooldownTimerNoCombat += Time.deltaTime;
            }
            else if (cooldownTimerNoCombat >= 2)
            {
                Cooldown();
                cooldownTimerNoCombat = 0;
            }
        }

        public void Cooldown()
        {
            if (surfaceEffectsList.Count > 0)
            {
                for (int i = 0; i < surfaceEffectsList.Count; i++)
                {
                    var surfaceEffect = surfaceEffectsList[i];
                    if (surfaceEffect.cooldownTimer > 0)
                    {
                        surfaceEffect.cooldownTimer -= 1;
                    }

                    if (surfaceEffect.cooldownTimer == 0)
                    {
                        RemoveSurfaceEffect(surfaceEffect, i--);
                    }
                }
            }
        }

        private void RemoveSurfaceEffect(SurfaceEffect surfaceEffect, int i)
        {
            Debug.Log($"Removing {surfaceEffect.name} surface effect");
            SFXPlayer.Instance.PlaySFX(surfaceEffect.removedAudioClip, surfaceEffect.transform.position, 20);
            if (surfaceEffect.removeVFX)
            {
                Instantiate(surfaceEffect.removeVFX, surfaceEffect.transform.position, Quaternion.identity);
            }

            Destroy(surfaceEffectsList[i].gameObject);
            surfaceEffectsList.RemoveAt(i--);
        }

        public void RemoveSurfaceEffect(SurfaceEffect surfaceEffect)
        {
            if (!surfaceEffect)
            {
                return;
            }
            
            Debug.Log($"Removing {surfaceEffect.name} surface effect");
            
            if (surfaceEffect.removedAudioClip)
            {
                SFXPlayer.Instance.PlaySFX(surfaceEffect.removedAudioClip, surfaceEffect.transform.position, 20);
            }

            if (surfaceEffect.removeVFX)
            {
                Instantiate(surfaceEffect.removeVFX, surfaceEffect.transform.position, Quaternion.identity);
            }

            Destroy(surfaceEffect.gameObject);
            surfaceEffectsList.Remove(surfaceEffect);
        }
    }
}