using System.IO;
using VfxEditor.Parsing;
using VfxEditor.ScdFormat.Sound.Data;

namespace VfxEditor.ScdFormat {
    public class TrackFilterData : ScdTrackData {
        public readonly ParsedEnum<FilterType> Type = new( "Type" );
        public readonly ParsedFloat Frequency = new( "Frequency" );
        public readonly ParsedFloat InvQ = new( "InvQ" );
        public readonly ParsedFloat Gain = new( "Gain" );

        public override void Read( BinaryReader reader ) {
            Type.Read( reader );
            Frequency.Read( reader );
            InvQ.Read( reader );
            Gain.Read( reader );
        }

        public override void Write( BinaryWriter writer ) {
            Type.Write( writer );
            Frequency.Write( writer );
            InvQ.Write( writer );
            Gain.Write( writer );
        }

        public override void Draw() {
            Type.Draw( CommandManager.Scd );
            Frequency.Draw( CommandManager.Scd );
            InvQ.Draw( CommandManager.Scd );
            Gain.Draw( CommandManager.Scd );
        }
    }
}
