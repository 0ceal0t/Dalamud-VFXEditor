using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public override void Draw( string parentId ) {
            AttackTime.Draw( parentId, CommandManager.Scd );
            DecayTime.Draw( parentId, CommandManager.Scd );
            SustainLevel.Draw( parentId, CommandManager.Scd );
            ReleaseTime.Draw( parentId, CommandManager.Scd );
        }
    }
}
