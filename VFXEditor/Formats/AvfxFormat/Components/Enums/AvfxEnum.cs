using System;
using System.IO;
using VfxEditor.Formats.AvfxFormat.Components;
using VfxEditor.Parsing;

namespace VfxEditor.AvfxFormat {
    public class AvfxEnum<T> : AvfxLiteral<ParsedEnum<T>, T> where T : Enum {
        public AvfxEnum( string name, string avfxName, T value ) : base( avfxName, new( name, value ) ) { }

        public AvfxEnum( string name, string avfxName ) : base( avfxName, new( name ) ) { }

        public override void ReadContents( BinaryReader reader, int _ ) => Parsed.Read( reader ); // Ignore size here
    }
}
