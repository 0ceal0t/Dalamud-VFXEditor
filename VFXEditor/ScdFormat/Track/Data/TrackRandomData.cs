using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class TrackRandomData : ScdTrackData {
        public readonly ParsedFloat Upper = new( "Upper" );
        public readonly ParsedFloat Lower = new( "Lower" );

        public override void Read( BinaryReader reader ) {
            Upper.Read( reader );
            Lower.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            Upper.Write( writer );
            Lower.Write( writer );
        }

        public override void Draw( string parentId ) {
            Upper.Draw( parentId, CommandManager.Scd );
            Lower.Draw( parentId, CommandManager.Scd );
        }
    }
}
