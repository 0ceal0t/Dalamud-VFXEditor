using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Part {
    public class UldParts {
        public readonly ParsedUInt Id = new( "Id" );

        private readonly List<UldPart> Parts = new();
        private int Offset => 12 + Parts.Count * 12;

        public UldParts( BinaryReader reader ) {
            Id.Read( reader );
            var partCount = reader.ReadInt32();
            reader.ReadInt32(); // skip offset
            for( var i = 0; i < partCount; i++ ) {
                Parts.Add( new UldPart( reader ) );
            }
        }

        public void Write( BinaryWriter writer ) {
            Id.Write( writer );
            writer.Write( Parts.Count );
            writer.Write( Offset );
            foreach( var part in Parts ) part.Write( writer );
        }
    }

    public class UldPart {
        private readonly ParsedUInt TextureId = new( "Texture Id" );
        private readonly ParsedUInt U = new( "U", size: 2 );
        private readonly ParsedUInt V = new( "V", size: 2 );
        private readonly ParsedUInt W = new( "W", size: 2 );
        private readonly ParsedUInt H = new( "H", size: 2 );

        public UldPart( BinaryReader reader ) {
            TextureId.Read( reader );
            U.Read( reader );
            V.Read( reader );
            W.Read( reader );
            H.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            TextureId.Write( writer );
            U.Write( writer );
            V.Write( writer );
            W.Write( writer );
            H.Write( writer );
        }
    }
}
