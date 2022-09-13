using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace intheclouds
{
    public enum StatusEffect
    {
        Burning, // cured by water, First Aid, Armour of Frost, or Fortify
        Bleeding, // cured by First Aid, Restoration
        Poisoned, // cured by Restoration, First Aid, or Fortify
        Blinded, // cured by First Aid
        Wet, // removed by burning, fire, chilled, frozen, shocked, or stunned
        Chilled, // removed by Burning
        Frozen, // cured by Burning
        Crippled, // cured by First Aid, Haste
        KnockedDown, // cured by First Aid
        Shocked, // removed by wet
        Stunned, // cured by Armour of Frost
        Silenced, // cured by First Aid
        Slowed, // cured by Haste
    }
}
