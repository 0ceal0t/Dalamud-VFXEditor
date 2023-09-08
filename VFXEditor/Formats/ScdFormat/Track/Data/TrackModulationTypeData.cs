using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class TrackModulationTypeData : ScdTrackData {
        public readonly ParsedEnum<OscillatorCarrier> Carrier = new( "Carrier" );
        public readonly ParsedEnum<OscillatorMode> Modulator = new( "Modulator" );

        public override void Read( BinaryReader reader ) {
            Carrier.Read( reader );
            Modulator.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            Carrier.Write( writer );
            Modulator.Write( writer );
        }

        public override void Draw() {
            Carrier.Draw( CommandManager.Scd );
            Modulator.Draw( CommandManager.Scd );
        }
    }
}
