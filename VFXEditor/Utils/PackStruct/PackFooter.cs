using System;
using System.IO;

namespace VFXEditor.Utils.PackStruct {
    // https://github.com/Ottermandias/Penumbra.GameData/blob/0e973ed6eace6afd31cd298f8c58f76fa8d5ef60/Files/PackStructs/PackFooter.cs

    public unsafe struct PackFooter {
        public const uint PackType = 'P' | ( ( uint )'A' << 8 ) | ( ( uint )'C' << 16 ) | ( ( uint )'K' << 24 );
        public PackHeader Header;
        public ulong TotalSize;

        public static bool TryRead( BinaryReader reader, out PackFooter packFooter ) {
            var requiredSize = sizeof( PackFooter );
            if( reader.BaseStream.Length < requiredSize ) {
                packFooter = default;
                return false;
            }

            reader.BaseStream.Position = reader.BaseStream.Length - requiredSize;
            packFooter = new SpanBinaryReader( reader.ReadBytes( requiredSize ) ).Read<PackFooter>();
            return packFooter.Header.Type == PackType;
        }


    }
}
