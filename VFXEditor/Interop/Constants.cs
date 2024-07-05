namespace VfxEditor.Interop {
    public static class Constants {
        public const string ReadFileSig = "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 41 54 41 55 41 56 41 57 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 48 63 42";
        public const string ReadSqpackSig = "40 56 41 56 48 83 EC ?? 0F BE 02 ";
        public const string GetResourceSyncSig = "E8 ?? ?? ?? ?? 48 8B D8 8B C7 ";
        public const string GetResourceAsyncSig = "E8 ?? ?? ?? 00 48 8B D8 EB ?? F0 FF 83 ?? ?? 00 00";

        public const string StaticVfxCreateSig = "E8 ?? ?? ?? ?? F3 0F 10 35 ?? ?? ?? ?? 48 89 43 08";
        public const string StaticVfxRunSig = "E8 ?? ?? ?? ?? 8B 4B 7C 85 C9";
        public const string StaticVfxRemoveSig = "40 53 48 83 EC 20 48 8B D9 48 8B 89 ?? ?? ?? ?? 48 85 C9 74 28 33 D2 E8 ?? ?? ?? ?? 48 8B 8B ?? ?? ?? ?? 48 85 C9";

        public const string ActorVfxCreateSig = "40 53 55 56 57 48 81 EC ?? ?? ?? ?? 0F 29 B4 24 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 0F B6 AC 24 ?? ?? ?? ?? 0F 28 F3 49 8B F8";
        public const string ActorVfxRemoveSig = "0F 11 48 10 48 8D 05"; // the weird one

        public const string CallTriggerSig = "E8 ?? ?? ?? ?? 0F B7 43 56";

        public const string GetMatrixSig = "E8 ?? ?? ?? ?? 48 8D 4C 24 ?? 48 89 4c 24 ?? 4C 8D 4D ?? 4C 8D 44 24 ??";
        public const string GetFileManagerSig = "48 8B 05 ?? ?? ?? ?? 48 85 C0 74 04 C6 40 6C 01";
        public const string GetFileManager2Sig = "E8 ?? ?? ?? ?? 48 8B 1D ?? ?? ?? ?? 48 8B CB 48 89 5C 24 ??";

        public const string DecRefSig = "E8 ?? ?? ?? ?? 48 C7 03 ?? ?? ?? ?? C6 83";
        public const string RequestFileSig = "E8 ?? ?? ?? ?? F0 FF 4E 5C";

        public const string ResourceManagerSig = "48 8B 0D ?? ?? ?? ?? 0F 5B F6";

        // https://github.com/Ottermandias/Penumbra.GameData/blob/main/Signatures.cs

        public const string CheckFileStateSig = "E8 ?? ?? ?? ?? 48 85 C0 74 ?? 4C 8B C8 ";

        public const string LoadTexFileLocalSig = "48 89 5C 24 08 48 89 6C 24 10 48 89 74 24 18 57 48 83 EC 30 49 8B F0 44 88 4C 24 20";
        public const string LodConfigSig = "48 8B 05 ?? ?? ?? ?? B3";
        public const string TexResourceHandleOnLoadSig = "40 53 55 41 54 41 55 41 56 41 57 48 81 EC ?? ?? ?? ?? 48 8B D9";

        public const string LoadMdlFileLocalSig = "48 89 5C 24 ?? 55 56 57 41 54 41 55 41 56 41 57 48 8D 6C 24 ?? 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 45 ?? 48 8B 72";
        public const string LoadMdlFileExternSig = "E8 ?? ?? ?? ?? EB 02 B0 F1";

        public const string PlayActionSig = "E8 ?? ?? ?? ?? 84 C0 75 04 32 DB EB 14";

        // https://github.com/aers/FFXIVClientStructs/blob/main/FFXIVClientStructs/FFXIV/Client/Game/Object/GameObject.cs

        public const int GameResourceOffset = 0x38;

        public const int PapIdsOffset = 0xF0;

        // https://github.com/imchillin/Anamnesis/blob/master/Anamnesis/Memory/ActorMemory.cs

        public const int PrepPapOffset = 105;

        public const byte PrepPapValue = 0xEC;

        // https://github.com/Ottermandias/Penumbra.GameData/blob/main/Offsets.cs

        public const int GetGameObjectIdxVfunc = 28;

        public const int TimelineToActionOffset = 152;

        public const string LuaManagerSig = "48 8B 0D ?? ?? ?? ?? BA 0F 00 00 10";

        public const string LuaActorVariableSig = "4C 8D 0D ?? ?? ?? ?? B9 ?? ?? ?? ?? 66 90 48 8B D1 48 D1 EA 4C 8B C2";

        public const string LuaGetVariableSig = "44 8B C2 C1 FA 1C 41 81 E0 FF FF FF 0F";

        public const string LuaReadSig = "E8 ?? ?? ?? ?? 89 03 B0 01 48 8B 5C 24 ?? 48 83 C4 20";

        // ========

        public const string HavokInterleavedVtblSig = "48 89 07 48 8B CD 48 89 77 38";

        public const string HavokMapperVtblSig = "48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 48 8D 05 ?? ?? ?? ?? C7 41 ?? ?? ?? ?? ?? 48 89 01 48 8B F1 48 83 C1 10 48 8B FA";

        public const string HavokSplineCtorSig = "48 89 5C 24 ?? 57 48 83 EC 40 48 8B DA 48 8B F9 E8 ?? ?? ?? ?? 48 8D 05 ?? ?? ?? ??";

        // https://github.com/lmcintyre/Dalamud.FindAnything/blob/a093b2f9e0c20e7d0479c091125ccca5ea09d683/Dalamud.FindAnything/Game/GameWindow.cs#L250

        public const string PlaySoundSig = "E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? FE C2";

        public const string InitSoundSig = "E8 ?? ?? ?? ?? 8B 5D 77";
    }
}
