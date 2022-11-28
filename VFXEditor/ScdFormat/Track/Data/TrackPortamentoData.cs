using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class TrackPortamentoData : ScdTrackData {
        public readonly ParsedInt PortamentoTime = new( "Portamento Time" );
        public readonly ParsedFloat Pitch = new( "Pitch" );

        public override void Read( BinaryReader reader ) {
            PortamentoTime.Read( reader );
            Pitch.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            PortamentoTime.Write( writer );
            Pitch.Write( writer );
        }

        public override void Draw( string parentId ) {
            PortamentoTime.Draw( parentId, CommandManager.Scd );
            Pitch.Draw( parentId, CommandManager.Scd );
        }
    }
}
