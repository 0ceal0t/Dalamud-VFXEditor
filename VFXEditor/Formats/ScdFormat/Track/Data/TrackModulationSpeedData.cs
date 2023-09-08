using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class TrackModulationSpeedData : ScdTrackData {
        public readonly ParsedInt Carrier = new( "Carrier" );
        public readonly ParsedInt Speed = new( "Speed" );

        public override void Read( BinaryReader reader ) {
            Carrier.Read( reader );
            Speed.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            Carrier.Write( writer );
            Speed.Write( writer );
        }

        public override void Draw() {
            Carrier.Draw( CommandManager.Scd );
            Speed.Draw( CommandManager.Scd );
        }
    }
}
