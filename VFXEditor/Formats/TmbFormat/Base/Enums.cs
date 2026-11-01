using System;

namespace VfxEditor.Formats.TmbFormat.Base {
    [Flags]
    public enum C010Unk2Flags {
        Unknown_1A = 0x01,
        Unknown_1B = 0x02,
        Unknown_1C = 0x04,
        Unknown_1D = 0x08,
        Unknown_1E = 0x10,
        Unknown_1F = 0x20,
        Unknown_1G = 0x40
    }

    [Flags]
    public enum C010Unk3Flags {
        Unknown_2A = 0x01,
        Unknown_2B = 0x02,
        Unknown_2C = 0x04,
        Unknown_2D = 0x08,
        Unknown_2E = 0x10,
        Unknown_2F = 0x20,
        Unknown_2G = 0x40
    }

    [Flags]
    public enum VfxC012Visibility {
        Use_Triggers = 0x01,
        Override_Battle_FX_Setting = 0x02,
        Unknown = 0x04,
    }

    [Flags]
    public enum SoundC053Flags {
        Stop_On_Animation_End = 0x01,
        Use_Bind_Id = 0x02,
        Unknown_1 = 0x04,
        Unknown_2 = 0x08,
        //x04-x80 unused, but keeping up to x08 in case
    }

    [Flags]
    public enum SoundC063Flags {
        Use_Min_Max_Range = 0x01,
        Stop_On_Animation_End = 0x02,
        Use_Bind_Id = 0x04,
    }

    [Flags]
    public enum InvisibilityFilter {
        Character = 0x01,
        Weapon = 0x02,
        OffHand = 0x04,
        Summon = 0x08,
        Unknown_2 = 0x10,
        Unknown_3 = 0x20,
        Unknown_4 = 0x40,
        Unknown_5 = 0x80,
    }

    public enum BindUser {
        Disabled = -1,
        Default,
        Caster,
        Target,
    }
    public enum BindType {
        Disabled = -1,
        Character,
        Weapon,
        Offhand,
        Summon_or_Lemure_0
        //Summon_or_Lemure_1
    }

    public enum C043Type {
        Invalid,
        Remove_Attribute,
        Apply_Attribute,
        Use_Internal_Id,
        Dead_Pose_Unknown = 5,
        Use_Weapon_Id = 8
    }

    public enum TerrainShape {
        Cone,
        Sphere,
        Unknown_1,
        Unknown_2
    }

    public enum PhysicsType {
        All,
        Equipment,
        Unknown_1, //j_ex_wing, Meteion
        Weapons,
        Hair,
        Chest, //j_mune
        Unknown_2 //unused
    }
    
    public enum FreezePositionType {
        Target_FixedDirection,
        Target_UserFacingDirection,
        Return
    }

    public enum AtchState {
        Stowed_State1,
        Drawn_State0,
        CraftGather_State2,
        SwitchHand_State3,
        n_throw_State4,
        SwitchReverse_State5
    }

    public enum C198AtchState {
        Default,
        Drawn_State0,
        Stowed_State1,
        CraftGather_State2,
        SwitchHand_State3,
        n_throw_State4,
        SwitchReverse_State5,
    }

    public enum ObjectControlFinal {
        Stowed_State1,
        Drawn_State0,
        CraftGather_State2,
        SwitchHand_State3,
        n_throw_State4,
        SwitchReverse_State5,
        Default,
    }

    public enum ObjectControl {
        Weapon_or_Pet,
        OffHand,
        Summon_or_Lemure_0,
        Summon_or_Lemure_1,
    }

    public enum C187ObjectControl {
        Summon_or_Lemure_0,
        Weapon_or_Pet,
        Offhand
    }

    public enum SummonId {
        Summon_0,
        Summon_1,
    }

    public enum SpeakTmbType {
        Normal,
        Whisper,
        Shout,
        Unknown_1,
        Unknown_2
    }

    public enum ObjectRotation {
        Use_Bind_Id_Rotation,
        Use_Origin_Rotation,
        Caster_Align
    }

    public enum SubtitleType {
        BattleTalk_Window = 0,
        Bordered_Plaintext = 1,
    }
}