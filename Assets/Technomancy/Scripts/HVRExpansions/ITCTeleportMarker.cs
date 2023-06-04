using System.Collections;
using System.Collections.Generic;
using HighlightPlus;
using HurricaneVR.Framework.Core.Player;
using UnityEngine;

namespace intheclouds
{
    public class ITCTeleportMarker : HVRTeleportMarker
    {
        private HighlightEffect _backstabHighlightEffect;

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            if (_backstabHighlightEffect)
            {
                _backstabHighlightEffect.highlighted = false;
            }
        }

        public void OnTriggerEnterEvent(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("BackstabTrigger") && UserInventory.instance.IsHoldingBackstabWeapon())
            {
                var target = other.transform.GetComponentInParent<BaseStats>() as EnemyStats;
                if (target && target.isAlive)
                {
                    if (other.TryGetComponent(out HighlightEffect highlightEffect))
                    {
                        _backstabHighlightEffect = highlightEffect;
                        _backstabHighlightEffect.highlighted = true;
                    }
                }
            }
        }
        
        public void OnTriggerExitEvent(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("BackstabTrigger"))
            {
                var target = other.transform.GetComponentInParent<BaseStats>() as EnemyStats;
                if (target && target.isAlive && _backstabHighlightEffect)
                {
                    _backstabHighlightEffect.highlighted = false;
                    _backstabHighlightEffect = null;
                }
            }
        }
    }
}
