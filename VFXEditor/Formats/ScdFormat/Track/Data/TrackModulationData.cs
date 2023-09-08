using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public enum OscillatorCarrier {
        None,
        Volume,
        Pitch,
        Pan,
        FrPan
    }

    public enum OscillatorMode {
        None,
        Sine,
        Rectangle,
        Triangle,
        Saw,
        Random,
        ReverseSine,
        ReverseRectangle,
        ReverseTriangle,
        ReverseSaw
    }

    public enum VolumeCurveType {
        Auto,
        Square,
        White,
        Old,
        OldWiiEmu,
        WhiteOnlyMusic,
        Caelum
    }

    public class TrackModulationData : ScdTrackData {
        public readonly ParsedEnum<OscillatorCarrier> Carrier = new( "Carrier", size: 1 );
        public readonly ParsedEnum<OscillatorMode> Modulator = new( "Modulator", size: 1 );
        public readonly ParsedEnum<VolumeCurveType> Curve = new( "Curve", size: 1 );
        private byte Reserved1;
        public readonly ParsedFloat Depth = new( "Depth" );
        public readonly ParsedInt Rate = new( "Rate" );

        public override void Read( BinaryReader reader ) {
            Carrier.Read( reader );
            Modulator.Read( reader );
            Curve.Read( reader );
            Reserved1 = reader.ReadByte();
            Depth.Read( reader );
            Rate.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            Carrier.Write( writer );
            Modulator.Write( writer );
            Curve.Write( writer );
            writer.Write( Reserved1 );
            Depth.Write( writer );
            Rate.Write( writer );
        }

        public override void Draw() {
            Carrier.Draw( CommandManager.Scd );
            Modulator.Draw( CommandManager.Scd );
            Curve.Draw( CommandManager.Scd );
            Depth.Draw( CommandManager.Scd );
            Rate.Draw( CommandManager.Scd );
        }
    }
}
