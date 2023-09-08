using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class TrackExternalAudioData : ScdTrackData {
        public ParsedShort BankNumber = new( "Bank Number" );
        private short Index = 0;
        private readonly List<short> RandomIndices = new();

        public override void Read( BinaryReader reader ) {
            BankNumber.Read( reader );
            Index = reader.ReadInt16();
            if( Index < 0 ) {
                for( var i = 0; i < ( -Index ); i++ ) RandomIndices.Add( reader.ReadInt16() );
            }
        }

        public override void Write( BinaryWriter writer ) {
            BankNumber.Write( writer );
            writer.Write( Index );
            foreach( var idx in RandomIndices ) writer.Write( idx );
        }

        public override void Draw() {
            BankNumber.Draw( CommandManager.Scd );
        }
    }
}
