using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VfxEditor.Parsing {
    public class ParsedSheetInt : ParsedSheetEnum<int> {
        public ParsedSheetInt( string name, Dictionary<int, string> sheetData, Action initData ) : base( name, sheetData, initData ) { }

        public override void Read( BinaryReader reader, int size ) => Read( reader );

        public override void Read( BinaryReader reader ) {
            Value = reader.ReadInt32();
        }

        public override void Write( BinaryWriter writer ) => writer.Write( Value );
    }
}