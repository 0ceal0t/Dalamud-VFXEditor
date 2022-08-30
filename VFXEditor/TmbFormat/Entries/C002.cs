using ImGuiNET;
using VFXEditor.TmbFormat.Utils;

namespace VFXEditor.TmbFormat.Entries {
    public class C002 : TmbEntry {
        public const string MAGIC = "C002";
        public override string DisplayName => "TMB (C002)";
        public override string Magic => MAGIC;

        public override int Size => 0x1C;
        public override int ExtraSize => 0;

        private int Duration = 50;
        private int Unk1 = 0;
        private int Unk2 = 0;
        private string Path = "";

        public C002() : base() { }

        public C002( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Duration = reader.ReadInt32();
            Unk1 = reader.ReadInt32();
            Unk2 = reader.ReadInt32();
            Path = reader.ReadOffsetString();
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            writer.Write( Duration );
            writer.Write( Unk1 );
            writer.Write( Unk2 );
            writer.WriteOffsetString( Path );
        }

        public override void Draw( string id ) {
            DrawHeader( id );
            ImGui.InputInt( $"Duration{id}", ref Duration );
            ImGui.InputInt( $"Unknown 1{id}", ref Unk1 );
            ImGui.InputInt( $"Unknown 2{id}", ref Unk2 );
            ImGui.InputText( $"Path{id}", ref Path, 255 );
        }
    }
}
