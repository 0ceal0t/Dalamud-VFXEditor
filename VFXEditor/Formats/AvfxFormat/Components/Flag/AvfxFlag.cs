using System;
using System.IO;
using VfxEditor.Formats.AvfxFormat.Components;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat {
    public class AvfxFlag<T> : AvfxLiteral<ParsedFlag<T>, T> where T : Enum {
        public AvfxFlag( string name, string avfxName, T value, int size = 4 ) : base( avfxName, new( name, value, size ) ) { }

        public AvfxFlag( string name, string avfxName, int size = 4 ) : base( avfxName, new( name, size: size ) ) { }

        public override void ReadContents( BinaryReader reader, int _ ) => Parsed.Read( reader ); // Ignore size here
    }
}
