using System;
using System.IO;

namespace VfxEditor.Utils.PackStruct {
    // https://github.com/Ottermandias/Penumbra.GameData/blob/0e973ed6eace6afd31cd298f8c58f76fa8d5ef60/Files/PackStructs/Pack.cs

    public ref struct Pack {
        public PackHeader Header;

        public ReadOnlySpan<byte> Data;

        public static unsafe void Write( BinaryWriter writer, uint type, ushort version, ReadOnlySpan<byte> data ) {
            writer.Write( type );
            writer.Write( version );
            writer.Write( ( ushort )0 );
            writer.Write( ( long )data.Length );
            writer.Write( data );
            writer.Write( PackFooter.PackType );
            writer.Write( version );
            writer.Write( ( ushort )1 );
            long offset = sizeof( PackHeader ) + data.Length;
            writer.Write( -offset );
            writer.Write( ( ulong )( offset + sizeof( PackHeader ) + sizeof( ulong ) ) );
        }
    }
}
