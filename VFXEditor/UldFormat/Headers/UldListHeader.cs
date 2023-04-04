using System;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.UldFormat.Headers {
    public class UldListHeader : UldGenericHeader {
        public uint ElementCount;
        private readonly ParsedInt Unknown1 = new( "Unknown 1" );

        public UldListHeader( BinaryReader reader ) : base( reader ) {
            ElementCount = reader.ReadUInt32();
            Unknown1.Read( reader );
        }

        public void Write( BinaryWriter writer, uint elementCount ) {
            ElementCount = elementCount;

            WriteHeader( writer );
            writer.Write( ElementCount );
            Unknown1.Write( writer );
        }
    }
}
