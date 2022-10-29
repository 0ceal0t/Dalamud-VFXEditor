using ImGuiNET;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C009 : TmbEntry {
        public const string MAGIC = "C009";
        public const string DISPLAY_NAME = "Animation - PAP Only (C009)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x18;
        public override int ExtraSize => 0;

        private int Duration = 50;
        private int Unk1 = 0;
        private string Path = "";

        public C009() : base() { }

        public C009( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Duration = reader.ReadInt32();
            Unk1 = reader.ReadInt32();
            Path = reader.ReadOffsetString();
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            writer.Write( Duration );
            writer.Write( Unk1 );
            writer.WriteOffsetString( Path );
        }

        public override void Draw( string id ) {
            DrawHeader( id );
            ImGui.InputInt( $"Duration{id}", ref Duration );
            ImGui.InputInt( $"Unknown 1{id}", ref Unk1 );
            ImGui.InputText( $"Path{id}", ref Path, 31 );
        }
    }
}
