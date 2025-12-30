namespace VFXEditor.Utils.PackStruct {
    // https://github.com/Ottermandias/Penumbra.GameData/blob/0e973ed6eace6afd31cd298f8c58f76fa8d5ef60/Files/PackStructs/PackHeader.cs

    public struct PackHeader {
        public uint Type;
        public ushort Version;
        public ushort PackCount;
        public long PriorOffset;
    }
}
