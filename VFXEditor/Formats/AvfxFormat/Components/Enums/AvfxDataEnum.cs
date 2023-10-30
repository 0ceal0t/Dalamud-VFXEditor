using System;
using System.IO;
using VfxEditor.AvfxFormat;
using VfxEditor.Formats.AvfxFormat.Nodes;
using VfxEditor.Parsing.Data;

namespace VfxEditor.Formats.AvfxFormat.Components.Enums {
    public class AvfxDataEnum<T> : AvfxLiteral<ParsedDataEnum<T, AvfxData>, T> where T : Enum {
        public AvfxDataEnum( AvfxNodeWithData<T> item, string name, string avfxName, T value ) : base( avfxName, new( item, name, value ) ) { }

        public AvfxDataEnum( AvfxNodeWithData<T> item, string name, string avfxName ) : base( avfxName, new( item, name ) ) { }

        public override void ReadContents( BinaryReader reader, int _ ) => Parsed.Read( reader ); // Ignore size here
    }
}
