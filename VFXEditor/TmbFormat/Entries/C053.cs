using ImGuiNET;
using VFXEditor.Helper;
using VFXEditor.TmbFormat.Utils;

namespace VFXEditor.TmbFormat.Entries {
    public class C053 : TmbEntry {
        public const string MAGIC = "C053";
        public override string DisplayName => "C053";
        public override string Magic => MAGIC;

        public override int Size => 0x1C;
        public override int ExtraSize => 0;

        private int Unk1 = 0;
        private int Unk2 = 0;
        private short Unk3 = 0;
        private short Unk4 = 0;
        private int Unk5 = 0;

        public C053() : base() { }

        public C053( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Unk1 = reader.ReadInt32();
            Unk2 = reader.ReadInt32();
            Unk3 = reader.ReadInt16();
            Unk4 = reader.ReadInt16();
            Unk5 = reader.ReadInt32();
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            writer.Write( Unk1 );
            writer.Write( Unk2 );
            writer.Write( Unk3 );
            writer.Write( Unk4 );
            writer.Write( Unk5 );
        }

        public override void Draw( string id ) {
            DrawHeader( id );
            ImGui.InputInt( $"Unknown 1{id}", ref Unk1 );
            ImGui.InputInt( $"Unknown 2{id}", ref Unk2 );
            FileHelper.ShortInput( $"Unknown 3{id}", ref Unk3 );
            FileHelper.ShortInput( $"Unknown 4{id}", ref Unk4 );
            ImGui.InputInt( $"Unknown 5{id}", ref Unk5 );
        }
    }
}
