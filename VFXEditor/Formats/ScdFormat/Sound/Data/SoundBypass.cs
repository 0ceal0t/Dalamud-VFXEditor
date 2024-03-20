using Dalamud.Interface.Utility.Raii;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.Formats.ScdFormat.Sound.Data {
    public class SoundBypass {
        private readonly ParsedShort Unknown1 = new( "Unknown 1" );
        private readonly ParsedShort Unknown2 = new( "Unknown 2" );
        private readonly ParsedUInt Unknown3 = new( "Unknown 3" );
        private readonly ParsedFloat Unknown4 = new( "Unknown 4" );
        private readonly ParsedUInt Unknown5 = new( "Unknown 5" );
        private readonly ParsedUInt Unknown6 = new( "Unknown 6" );
        private readonly ParsedFloat Unknown7 = new( "Unknown 7" );
        private readonly ParsedUInt Unknown8 = new( "Unknown 8" );
        private readonly ParsedUInt Unknown9 = new( "Unknown 9" );

        public void Read( BinaryReader reader ) {
            Unknown1.Read( reader );
            Unknown2.Read( reader );
            Unknown3.Read( reader );
            Unknown4.Read( reader );
            Unknown5.Read( reader );
            Unknown6.Read( reader );
            Unknown7.Read( reader );
            Unknown8.Read( reader );
            Unknown9.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Unknown1.Write( writer );
            Unknown2.Write( writer );
            Unknown3.Write( writer );
            Unknown4.Write( writer );
            Unknown5.Write( writer );
            Unknown6.Write( writer );
            Unknown7.Write( writer );
            Unknown8.Write( writer );
            Unknown9.Write( writer );
        }

        public void Draw() {
            using var _ = ImRaii.PushId( "Unknown" );

            Unknown1.Draw();
            Unknown2.Draw();
            Unknown3.Draw();
            Unknown4.Draw();
            Unknown5.Draw();
            Unknown6.Draw();
            Unknown7.Draw();
            Unknown8.Draw();
            Unknown9.Draw();
        }
    }
}
