using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class TrackModulationDepthFadeData : ScdTrackData {
        public readonly ParsedInt Carrier = new( "Carrier" );
        public readonly ParsedFloat Depth = new( "Depth" );
        public readonly ParsedInt FadeTime = new( "Fade Time" );

        public override void Read( BinaryReader reader ) {
            Carrier.Read( reader );
            Depth.Read( reader );
            FadeTime.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            Carrier.Write( writer );
            Depth.Write( writer );
            FadeTime.Write( writer );
        }

        public override void Draw( string parentId ) {
            Carrier.Draw( parentId, CommandManager.Scd );
            Depth.Draw( parentId, CommandManager.Scd );
            FadeTime.Draw( parentId, CommandManager.Scd );
        }
    }
}
