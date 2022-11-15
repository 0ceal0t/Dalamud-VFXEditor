using VfxEditor.Utils;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Parsing;

namespace VfxEditor.TmbFormat {
    public class Tmdh : TmbItemWithId {
        public override string Magic => "TMDH";
        public override int Size => 0x10;
        public override int ExtraSize => 0;

        private readonly ParsedInt Unk1 = new( "Unknown 1", size: 2 );
        private readonly ParsedInt Unk2 = new( "Unknown 2", size: 2 );
        private readonly ParsedInt Unk3 = new( "Unknown 3", size: 2 );

        public Tmdh( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Unk1.Read( reader.Reader, 2 );
            Unk2.Read( reader.Reader, 2 );
            Unk3.Read( reader.Reader, 2 );
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            Unk1.Write( writer.Writer );
            Unk2.Write( writer.Writer );
            Unk3.Write( writer.Writer );
        }

        public void Draw( string id ) {
            Unk1.Draw( id, CommandManager.Tmb );
            Unk2.Draw( id, CommandManager.Tmb );
            Unk3.Draw( id, CommandManager.Tmb );
        }
    }
}
