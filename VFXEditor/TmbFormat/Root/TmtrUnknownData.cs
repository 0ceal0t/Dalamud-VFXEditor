using System.IO;
using VfxEditor.Parsing;

namespace VfxEditor.TmbFormat {
    public class TmtrUnknownData {
        public readonly TmbFile File;

        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedShort Unk2 = new( "Unknown 2" );
        private readonly ParsedShort Unk3 = new( "Unknown 3" );
        private readonly ParsedInt Unk4 = new( "Unknown 4" );

        public TmtrUnknownData( TmbFile file ) {
            File = file;
        }

        public TmtrUnknownData( BinaryReader reader, TmbFile file ) : this( file ) {
            Unk1.Read( reader );
            Unk2.Read( reader );
            Unk3.Read( reader );
            Unk4.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            Unk1.Write( writer );
            Unk2.Write( writer );
            Unk3.Write( writer );
            Unk4.Write( writer );
        }

        public void Draw() {
            Unk1.Draw( File.Command );
            Unk2.Draw( File.Command );
            Unk3.Draw( File.Command );
        }
    }
}
