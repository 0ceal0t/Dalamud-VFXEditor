using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class TrackModulationOffData : ScdTrackData {
        public readonly ParsedEnum<OscillatorCarrier> Carrier = new( "Carrier" );

        public override void Read( BinaryReader reader ) {
            Carrier.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            Carrier.Write( writer );
        }

        public override void Draw( string parentId ) {
            Carrier.Draw( parentId, CommandManager.Scd );
        }
    }
}
