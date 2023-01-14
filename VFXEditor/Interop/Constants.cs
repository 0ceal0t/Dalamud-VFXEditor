namespace VfxEditor.Interop {
    public static class Constants {
        public static readonly string ReadFileSig = "E8 ?? ?? ?? ?? 84 C0 0F 84 ?? 00 00 00 4C 8B C3 BA 05";
        public static readonly string ReadSqpackSig = "E8 ?? ?? ?? ?? EB 05 E8 ?? ?? ?? ?? 84 C0 0F 84 ?? 00 00 00 4C 8B C3";
        public static readonly string GetResourceSyncSig = "E8 ?? ?? 00 00 48 8D 8F ?? ?? 00 00 48 89 87 ?? ?? 00 00";
        public static readonly string GetResourceAsyncSig = "E8 ?? ?? ?? 00 48 8B D8 EB ?? F0 FF 83 ?? ?? 00 00";

        public static readonly string StaticVfxCreateSig = "E8 ?? ?? ?? ?? F3 0F 10 35 ?? ?? ?? ?? 48 89 43 08";
        public static readonly string StaticVfxRemoveSig = "40 53 48 83 EC 20 48 8B D9 48 8B 89 ?? ?? ?? ?? 48 85 C9 74 28 33 D2 E8 ?? ?? ?? ?? 48 8B 8B ?? ?? ?? ?? 48 85 C9";
        public static readonly string ActorVfxCreateSig = "40 53 55 56 57 48 81 EC ?? ?? ?? ?? 0F 29 B4 24 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 0F B6 AC 24 ?? ?? ?? ?? 0F 28 F3 49 8B F8";
        public static readonly string ActorVfxRemoveSig = "0F 11 48 10 48 8D 05"; // the weird one
        public static readonly string StaticVfxRunSig = "E8 ?? ?? ?? ?? 8B 4B 7C 85 C9";

        public static readonly string GetMatrixSig = "E8 ?? ?? ?? ?? 48 8D 4C 24 ?? 48 89 4c 24 ?? 4C 8D 4D ?? 4C 8D 44 24 ??";
        public static readonly string GetFileManagerSig = "48 8B 05 ?? ?? ?? ?? 48 85 C0 74 04 C6 40 6C 01";
        public static readonly string GetFileManager2Sig = "E8 ?? ?? ?? ?? 4C 8B 2D ?? ?? ?? ?? 49 8B CD";

        public static readonly string DecRefSig = "E8 ?? ?? ?? ?? 48 C7 03 ?? ?? ?? ?? C6 83";
        public static readonly string RequestFileSig = "E8 ?? ?? ?? ?? F0 FF 4F 5C 48 8D 4F 30";

        public static readonly string ResourceManagerSig = "48 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 32 C0";

        // https://github.com/aers/FFXIVClientStructs/blob/fe20b24789a5b2c3eaae1e02b974187769615353/FFXIVClientStructs/FFXIV/Client/Game/Object/GameObject.cs#L46
        public static readonly int RenderOffset = 0x114;

        public static readonly int GameResourceOffset = 0x38;

        public static readonly int PapIdsOffset = 0xf0;

        // https://github.com/imchillin/Anamnesis/blob/c092a5e7e224d45694a484ba2cd86155203960d2/Anamnesis/Memory/ActorMemory.cs
        public static readonly int ActorAnimationOffset = 0x0900;
    }
}
