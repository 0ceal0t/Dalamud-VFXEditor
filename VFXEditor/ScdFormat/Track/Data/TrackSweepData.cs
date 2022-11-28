using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class TrackSweepData : ScdTrackData {
        public readonly ParsedFloat Pitch = new( "Pitch" );
        public readonly ParsedInt Time = new( "Time" );

        public override void Read( BinaryReader reader ) {
            Pitch.Read( reader );
            Time.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            Pitch.Write( writer );
            Time.Write( writer );
        }

        public override void Draw( string parentId ) {
            Pitch.Draw( parentId, CommandManager.Scd );
            Time.Draw( parentId, CommandManager.Scd );
        }
    }
}
