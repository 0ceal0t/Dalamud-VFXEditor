using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public override void Draw( string parentId ) {
            BankNumber.Draw( parentId, CommandManager.Scd );
            SoundIndex.Draw( parentId, CommandManager.Scd );
        }
    }
}
