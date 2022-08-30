using ImGuiNET;
using VFXEditor.TmbFormat.Utils;

namespace VFXEditor.TmbFormat.Entries {
    public class C063 : TmbEntry {
        public const string MAGIC = "C063";
        public const string DISPLAY_NAME = "Sound (C063)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x20;
        public override int ExtraSize => 0;

        private int Unk1 = 1;
        private int Unk2 = 0;
        private string Path = "";
        private int SoundIndex = 1;
        private int Unk3 = 0;

        public C063() : base() { }

        public C063( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Unk1 = reader.ReadInt32();
            Unk2 = reader.ReadInt32();
            Path = reader.ReadOffsetString();
            SoundIndex = reader.ReadInt32();
            Unk3 = reader.ReadInt32();
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            writer.Write( Unk1 );
            writer.Write( Unk2 );
            writer.WriteOffsetString( Path );
            writer.Write( SoundIndex );
            writer.Write( Unk3 );
        }

        public override void Draw( string id ) {
            DrawHeader( id );
            ImGui.InputInt( $"Unknown 1{id}", ref Unk1 );
            ImGui.InputInt( $"Unknown 2{id}", ref Unk2 );
            ImGui.InputText( $"Path{id}", ref Path, 255 );
            ImGui.InputInt( $"Sound Index{id}", ref SoundIndex );
            ImGui.InputInt( $"Unknown 3{id}", ref Unk3 );
        }
    }
}
