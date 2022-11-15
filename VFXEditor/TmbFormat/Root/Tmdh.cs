using VfxEditor.TmbFormat.Utils;
using VfxEditor.Parsing;

namespace VfxEditor.TmbFormat {
    public class Tmdh : TmbItemWithId {
        public override string Magic => "TMDH";
        public override int Size => 0x10;
        public override int ExtraSize => 0;

        private readonly ParsedShort Unk1 = new( "Unknown 1" );
        private readonly ParsedShort Unk2 = new( "Unknown 2" );
        private readonly ParsedShort Unk3 = new( "Unknown 3" );

        public Tmdh( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Unk1.Read( reader );
            Unk2.Read( reader );
            Unk3.Read( reader );
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            Unk1.Write( writer );
            Unk2.Write( writer );
            Unk3.Write( writer );
        }

        public void Draw( string id ) {
            Unk1.Draw( id, CommandManager.Tmb );
            Unk2.Draw( id, CommandManager.Tmb );
            Unk3.Draw( id, CommandManager.Tmb );
        }
    }
}
