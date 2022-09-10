using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace intheclouds
{
    public class Fireball : Magic
    {
        private void OnDisable()
        {
            var player = GameManager.Instance.FindControlledPlayer().LocalUserObjects;
            var highlight = player.handAugmentHighlight;
            highlight.overlayColor = player.PlayerStats.statsSO.baseHandAugmentColor;
            highlight.SetGlowColor(player.PlayerStats.statsSO.baseHandAugmentColor);
            highlight.highlighted = false;
        }
    }
}
