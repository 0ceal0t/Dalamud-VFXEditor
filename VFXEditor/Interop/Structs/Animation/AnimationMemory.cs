using System.Runtime.InteropServices;

namespace VfxEditor.Interop.Structs.Animation {
    public enum CharacterModes : byte {
        None = 0,
        Normal = 1,
        EmoteLoop = 3,
        Mounted = 4,
        AnimLock = 8,
        Carrying = 9,
        InPositionLoop = 11,
        Performance = 16,
    }

    public enum AnimationSlots : int {
        FullBody = 0,
        UpperBody = 1,
        Facial = 2,
        Add = 3,
        Lips = 7,
        Parts1 = 8,
        Parts2 = 9,
        Parts3 = 10,
        Parts4 = 11,
        Overlay = 12,
    }

    // https://github.com/imchillin/Anamnesis/blob/master/Anamnesis/Memory/ActorMemory.cs

    [StructLayout( LayoutKind.Explicit )]
    public unsafe struct ActorMemoryStruct {
        [FieldOffset( 0x0A30 )] public AnimationMemory Animation;
        [FieldOffset( 0x2354 )] public byte CharacterMode;
        [FieldOffset( 0x2355 )] public byte CharacterModeInput;

        public readonly bool CanAnimate => ( CharacterModes )CharacterMode == CharacterModes.Normal || ( CharacterModes )CharacterMode == CharacterModes.AnimLock;
        public readonly bool IsAnimationOverride => ( CharacterModes )CharacterMode == CharacterModes.AnimLock;
    }

    // https://github.com/imchillin/Anamnesis/blob/master/Anamnesis/Memory/AnimationMemory.cs

    [StructLayout( LayoutKind.Explicit )]
    public unsafe struct AnimationMemory {
        [FieldOffset( 0x0F0 )] public fixed ushort AnimationIds[13];
        [FieldOffset( 0x164 )] public fixed float Speeds[13];
        [FieldOffset( 0x1F2 )] public byte SpeedTrigger;
        [FieldOffset( 0x2E6 )] public ushort BaseOverride;
        [FieldOffset( 0x2E8 )] public ushort LipsOverride;
    }
}
