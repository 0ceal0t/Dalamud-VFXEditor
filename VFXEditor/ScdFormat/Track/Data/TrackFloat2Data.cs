using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class TrackFloat2Data : ScdTrackData {
        public readonly ParsedFloat2 Value = new( "Value" );

        public override void Read( BinaryReader reader ) {
            Value.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            Value.Write( writer );
        }

        public override void Draw( string parentId ) {
            Value.Draw( parentId, CommandManager.Scd );
        }
    }
}
