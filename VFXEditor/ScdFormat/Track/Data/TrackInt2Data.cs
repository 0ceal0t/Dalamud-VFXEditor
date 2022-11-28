using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class TrackInt2Data : ScdTrackData {
        public readonly ParsedInt Value1 = new( "Value 1" );
        public readonly ParsedInt Value2 = new( "Value 2" );

        public override void Read( BinaryReader reader ) {
            Value1.Read( reader );
            Value2.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            Value1.Write( writer );
            Value2.Write( writer );
        }

        public override void Draw( string parentId ) {
            Value1.Draw( parentId, CommandManager.Scd );
            Value2.Draw( parentId, CommandManager.Scd );
        }
    }
}
