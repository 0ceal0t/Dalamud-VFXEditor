using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class TrackPlayInnerSoundData : ScdTrackData {
        public readonly ParsedShort BankNumber = new( "Bank Number" );
        public readonly ParsedShort SoundIndex = new( "Sound Index" );

        public override void Read( BinaryReader reader ) {
            BankNumber.Read( reader );
            SoundIndex.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            BankNumber.Write( writer );
            SoundIndex.Write( writer );
        }

        public override void Draw() {
            BankNumber.Draw();
            SoundIndex.Draw();
        }
    }
}
