using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class TrackModulationDepthData : ScdTrackData {
        public readonly ParsedInt Carrier = new( "Carrier" );
        public readonly ParsedFloat Depth = new( "Depth" );

        public override void Read( BinaryReader reader ) {
            Carrier.Read( reader );
            Depth.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            Carrier.Write( writer );
            Depth.Write( writer );
        }

        public override void Draw( string parentId ) {
            Carrier.Draw( parentId, CommandManager.Scd );
            Depth.Draw( parentId, CommandManager.Scd );
        }
    }
}
