using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Utils;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat {
    public class TmbOffsetString : ParsedString {
        public TmbOffsetString( string name, List<ParsedStringIcon> icons, bool forceLower ) : base( name, icons, forceLower ) { }

        public TmbOffsetString( string name ) : base( name ) { }

        public override void Read( ParsingReader reader ) {
            if( reader is TmbReader tmbReader ) {
                Value = tmbReader.ReadOffsetString();
            }
        }

        public override void Write( ParsingWriter writer ) {
            if( writer is TmbWriter tmbWriter ) {
                tmbWriter.WriteOffsetString( Value );
            }
        }
    }
}
