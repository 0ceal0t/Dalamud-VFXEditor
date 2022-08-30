using VFXEditor.Helper;
using VFXEditor.TmbFormat.Utils;

namespace VFXEditor.TmbFormat {
    public class Tmdh : TmbItemWithId {
        public override string Magic => "TMDH";
        public override int Size => 0x10;
        public override int ExtraSize => 0;

        private short Unk1;
        private short Unk2;
        private short Unk3;

        public Tmdh( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Unk1 = reader.ReadInt16();
            Unk2 = reader.ReadInt16(); // ?
            Unk3 = reader.ReadInt16(); // 3
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            writer.Write( Unk1 );
            writer.Write( Unk2 );
            writer.Write( Unk3 );
        }

        public void Draw( string id ) {
            FileHelper.ShortInput( $"Unknown 1{id}", ref Unk1 );
            FileHelper.ShortInput( $"Unknown 2{id}", ref Unk2 );
            FileHelper.ShortInput( $"Unknown 3{id}", ref Unk3 );
        }
    }
}
