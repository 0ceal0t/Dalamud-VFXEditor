namespace VfxEditor.Interop {
    public static class Constants {
        public const string ReadFileSig = "E8 ?? ?? ?? ?? 84 C0 0F 84 ?? 00 00 00 4C 8B C3 BA 05";
        public const string ReadSqpackSig = "E8 ?? ?? ?? ?? EB 05 E8 ?? ?? ?? ?? 84 C0 0F 84 ?? 00 00 00 4C 8B C3";
        public const string GetResourceSyncSig = "E8 ?? ?? 00 00 48 8D 8F ?? ?? 00 00 48 89 87 ?? ?? 00 00";
        public const string GetResourceAsyncSig = "E8 ?? ?? ?? 00 48 8B D8 EB ?? F0 FF 83 ?? ?? 00 00";

        public const string StaticVfxCreateSig = "E8 ?? ?? ?? ?? F3 0F 10 35 ?? ?? ?? ?? 48 89 43 08";
        public const string StaticVfxRunSig = "E8 ?? ?? ?? ?? 8B 4B 7C 85 C9";
        public const string StaticVfxRemoveSig = "40 53 48 83 EC 20 48 8B D9 48 8B 89 ?? ?? ?? ?? 48 85 C9 74 28 33 D2 E8 ?? ?? ?? ?? 48 8B 8B ?? ?? ?? ?? 48 85 C9";

        public const string ActorVfxCreateSig = "40 53 55 56 57 48 81 EC ?? ?? ?? ?? 0F 29 B4 24 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 0F B6 AC 24 ?? ?? ?? ?? 0F 28 F3 49 8B F8";
        public const string ActorVfxRemoveSig = "0F 11 48 10 48 8D 05"; // the weird one

        public const string CallTriggerSig = "E8 ?? ?? ?? ?? 44 88 63 53 48 8B 4B 30";

        public const string GetMatrixSig = "E8 ?? ?? ?? ?? 48 8D 4C 24 ?? 48 89 4c 24 ?? 4C 8D 4D ?? 4C 8D 44 24 ??";
        public const string GetFileManagerSig = "48 8B 05 ?? ?? ?? ?? 48 85 C0 74 04 C6 40 6C 01";
        public const string GetFileManager2Sig = "E8 ?? ?? ?? ?? 4C 8B 2D ?? ?? ?? ?? 49 8B CD";

        public const string DecRefSig = "E8 ?? ?? ?? ?? 48 C7 03 ?? ?? ?? ?? C6 83";
        public const string RequestFileSig = "E8 ?? ?? ?? ?? F0 FF 4F 5C 48 8D 4F 30";

        public const string ResourceManagerSig = "48 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 32 C0";

        // https://github.com/xivdev/Penumbra/blob/master/Penumbra.GameData/Signatures.cs

        public const string CheckFileStateSig = "E8 ?? ?? ?? ?? 48 85 c0 74 ?? 45 0f b6 ce 48 89 44 24";
        public const string LoadTexFileLocalSig = "48 89 5C 24 08 48 89 6C 24 10 48 89 74 24 18 57 48 83 EC 30 49 8B F0 44 88 4C 24 20";
        public const string LoadTexFileExternSig = "E8 ?? ?? ?? ?? 0F B6 E8 48 8B CB E8";

        public const string PlayActionSig = "E8 ?? ?? ?? ?? 84 C0 75 04 32 DB EB 14";

        // https://github.com/aers/FFXIVClientStructs/blob/main/FFXIVClientStructs/FFXIV/Client/Game/Object/GameObject.cs

        public const int RenderFlagOffset = 0x114;

        public const int GameResourceOffset = 0x38;

        public const int PapIdsOffset = 0xf0;

        // https://github.com/imchillin/Anamnesis/blob/master/Anamnesis/Memory/ActorMemory.cs

        public const int AnimationMemoryOffset = 0x0930;

        public const int PrepPapOffset = 105;

        public const byte PrepPapValue = 0xec;

        // https://github.com/xivdev/Penumbra/blob/master/Penumbra.GameData/Offsets.cs#L18

        public const int GetGameObjectIdxVfunc = 28;

        public const int TimelineToActionOffset = 152;

        public const string LuaManagerSig = "48 8B 0D ?? ?? ?? ?? BA 0F 00 00 10";

        public const string LuaActorVariableSig = "C7 05 ?? ?? ?? ?? 13 00 00 10 48 89 05 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ??";

        public const string LuaVariableSig = "44 8B C2 C1 FA 1C 41 81 E0 FF FF FF 0F";

        // ========

        public const string HavokInterleavedVtblSig = "48 89 07 48 8B CD 48 89 77 38";

        public const string HavokSplineCtorSig = "48 89 5C 24 ?? 57 48 83 EC 40 48 8B DA 48 8B F9 E8 ?? ?? ?? ?? 48 8D 05 ?? ?? ?? ??";

        // https://github.com/lmcintyre/Dalamud.FindAnything/blob/a093b2f9e0c20e7d0479c091125ccca5ea09d683/Dalamud.FindAnything/Game/GameWindow.cs#L250

        public const string PlaySoundSig = "E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? FE C2 ";

        public const string InitSoundSig = "E8 ?? ?? ?? ?? 8B 7D 77 85 FF";
    }
}
