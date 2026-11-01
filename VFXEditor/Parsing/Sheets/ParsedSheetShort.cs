using System;
using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Parsing {
    public class ParsedSheetShort : ParsedSheetEnum<ushort> {
        public ParsedSheetShort( string name, Dictionary<ushort, string> sheetData, Action initData ) : base( name, sheetData, initData ) { }

        public override void Read( BinaryReader reader, int size ) => Read( reader );

        public override void Read( BinaryReader reader ) {
            Value = reader.ReadUInt16();
        }

        public override void Write( BinaryWriter writer ) => writer.Write( Value );
    }
}