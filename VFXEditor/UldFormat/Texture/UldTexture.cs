using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Data {
    public class UldTexture {
        private readonly ParsedUInt Id = new( "Id" );
        private readonly ParsedString Path = new( "Path", maxSize: 44 );
        private readonly ParsedUInt Unk1 = new( "Unknown 1" );
        private readonly ParsedUInt Unk2 = new( "Unknown 2" );

        public UldTexture( BinaryReader reader, char minorVersion ) {
            Id.Read( reader );
            Path.Read( reader );
            Path.Pad( reader, 44 );
            Unk1.Read( reader );
            if( minorVersion == '1' ) Unk2.Read( reader );
            else Unk2.Value = 0;
        }

        public void Writer( BinaryWriter writer, char minorVersion ) {
            Id.Write( writer );
            Path.Write( writer );
            Path.Pad( writer, 44 );
            Unk1.Write( writer );
            if( minorVersion == '1' ) Unk2.Write( writer );
        }
    }
}
