using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class TrackModulationSpeedFadeData : ScdTrackData {
        public readonly ParsedInt Carrier = new( "Carrier" );
        public readonly ParsedInt Speed = new( "Speed" );
        public readonly ParsedInt FadeTime = new( "Fade Time" );

        public override void Read( BinaryReader reader ) {
            Carrier.Read( reader );
            Speed.Read( reader );
            FadeTime.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            Carrier.Write( writer );
            Speed.Write( writer );
            FadeTime.Write( writer );
        }

        public override void Draw() {
            Carrier.Draw();
            Speed.Draw();
            FadeTime.Draw();
        }
    }
}
