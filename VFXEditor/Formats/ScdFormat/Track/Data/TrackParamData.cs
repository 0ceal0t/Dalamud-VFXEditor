using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class TrackParamData : ScdTrackData {
        public readonly ParsedFloat Value = new( "Value" );
        public readonly ParsedInt Time = new( "Time" );

        public override void Read( BinaryReader reader ) {
            Value.Read( reader );
            Time.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            Value.Write( writer );
            Time.Write( writer );
        }

        public override void Draw() {
            Value.Draw();
            Time.Draw();
        }
    }
}
