using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class TrackModulationOffData : ScdTrackData {
        public readonly ParsedEnum<OscillatorCarrier> Carrier = new( "Carrier" );

        public override void Read( BinaryReader reader ) {
            Carrier.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            Carrier.Write( writer );
        }

        public override void Draw() {
            Carrier.Draw( CommandManager.Scd );
        }
    }
}
