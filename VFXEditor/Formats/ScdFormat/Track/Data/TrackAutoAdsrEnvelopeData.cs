using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class TrackAutoAdsrEnvelopeData : ScdTrackData {
        public readonly ParsedInt AttackTime = new( "Attack Time" );
        public readonly ParsedInt DecayTime = new( "Decay Time" );
        public readonly ParsedInt SustainLevel = new( "Sustain Level" );
        public readonly ParsedInt ReleaseTime = new( "Release Time" );

        public override void Read( BinaryReader reader ) {
            AttackTime.Read( reader );
            DecayTime.Read( reader );
            SustainLevel.Read( reader );
            ReleaseTime.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            AttackTime.Write( writer );
            DecayTime.Write( writer );
            SustainLevel.Write( writer );
            ReleaseTime.Write( writer );
        }

        public override void Draw() {
            AttackTime.Draw();
            DecayTime.Draw();
            SustainLevel.Draw();
            ReleaseTime.Draw();
        }
    }
}
