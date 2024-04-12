namespace VfxEditor.Interop.Penumbra {
    public enum EquipSlot : byte {
        Unknown = 0,
        MainHand = 1,
        OffHand = 2,
        Head = 3,
        Body = 4,
        Hands = 5,
        Belt = 6,
        Legs = 7,
        Feet = 8,
        Ears = 9,
        Neck = 10,
        Wrists = 11,
        RFinger = 12,
        BothHand = 13,
        LFinger = 14, // Not officially existing, means "weapon could be equipped in either hand" for the game.
        HeadBody = 15,
        BodyHandsLegsFeet = 16,
        SoulCrystal = 17,
        LegsFeet = 18,
        FullBody = 19,
        BodyHands = 20,
        BodyLegsFeet = 21,
        ChestHands = 22,
        Nothing = 23,
        All = 24, // Not officially existing
    }

    public enum ObjectType : byte {
        Unknown,
        Vfx,
        DemiHuman,
        Accessory,
        World,
        Housing,
        Monster,
        Icon,
        LoadingScreen,
        Map,
        Interface,
        Equipment,
        Character,
        Weapon,
        Font,
    }

    public enum BodySlot : byte {
        Unknown,
        Hair,
        Face,
        Tail,
        Body,
        Zear,
    }
}
