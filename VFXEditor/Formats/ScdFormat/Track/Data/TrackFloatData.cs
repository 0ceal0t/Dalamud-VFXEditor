using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class TrackFloatData : ScdTrackData {
        public readonly ParsedFloat Value = new( "Value" );

        public override void Read( BinaryReader reader ) {
            Value.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            Value.Write( writer );
        }

        public override void Draw() {
            Value.Draw();
        }
    }
}
