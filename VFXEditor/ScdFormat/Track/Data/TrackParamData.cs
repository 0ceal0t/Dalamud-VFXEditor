using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class TrackParamData : ScdTrackData {
        public readonly ParsedFloat Value = new( "Value" );
        public readonly ParsedInt Time = new( "Time" );

        public override void Read( BinaryReader reader ) {
            Value.Read( reader );
            Time.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            Value.Write( writer );
            Time.Write( writer );
        }

        public override void Draw( string parentId ) {
            Value.Draw( parentId, CommandManager.Scd );
            Time.Draw( parentId, CommandManager.Scd );
        }
    }
}
