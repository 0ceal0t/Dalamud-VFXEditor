using Dalamud.Interface.Utility.Raii;
using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.Formats.ScdFormat.Sound.Data {
    public class SoundEmptyLoop {
        private readonly ParsedInt Unknown1 = new( "Unknown 1" );
        private readonly ParsedInt Unknown2 = new( "Unknown 2" );

        public void Read( BinaryReader reader ) {
            Unknown1.Read( reader );
            Unknown2.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Unknown1.Write( writer );
            Unknown2.Write( writer );
        }

        public void Draw() {
            using var _ = ImRaii.PushId( "Unknown" );

            Unknown1.Draw();
            Unknown2.Draw();
        }
    }
}
