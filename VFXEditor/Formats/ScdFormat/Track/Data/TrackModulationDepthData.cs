using System.IO;
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

        public override void Draw() {
            Carrier.Draw( CommandManager.Scd );
            Depth.Draw( CommandManager.Scd );
        }
    }
}
