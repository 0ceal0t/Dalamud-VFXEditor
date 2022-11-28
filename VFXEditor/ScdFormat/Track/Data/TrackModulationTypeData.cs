using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VfxEditor.Parsing;

namespace VfxEditor.ScdFormat {
    public class TrackModulationTypeData : ScdTrackData {
        public readonly ParsedEnum<OscillatorCarrier> Carrier = new( "Carrier" );
        public readonly ParsedEnum<OscillatorMode> Modulator = new( "Modulator" );

        public override void Read( BinaryReader reader ) {
            Carrier.Read( reader );
            Modulator.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            Carrier.Write( writer );
            Modulator.Write( writer );
        }

        public override void Draw( string parentId ) {
            Carrier.Draw( parentId, CommandManager.Scd );
            Modulator.Draw( parentId, CommandManager.Scd );
        }
    }
}
