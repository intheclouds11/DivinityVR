namespace intheclouds
{
    public enum ActionType
    {
        Attack,
        Movement,
        Heal,
        Selection
    }
    
    public enum DamageType
    {
        Physical,
        Magic
    }

    public enum ScalingType
    {
        None,
        Pyrokinetic,
        Hydrosophist,
        Aerotheurge,
        Geomancer,
        Warfare,
        Huntsman,
        Scoundrel,
    }
    
    public enum StatusEffectType
    {
        None,
        Burning, // cured by water, First Aid, Armour of Frost, or Fortify
        Bleeding, // cured by First Aid, Restoration, Fortify
        Poison, // cured by Restoration, First Aid, or Fortify
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
        Regenerating,
        MagicShell,
        Fortify,
        FavorableWind,
    }
}