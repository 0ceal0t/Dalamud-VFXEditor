using ImGuiNET;
using VFXEditor.TmbFormat.Utils;

namespace VFXEditor.TmbFormat.Entries {
    public class C010 : TmbEntry {
        public const string MAGIC = "C010";
        public override string DisplayName => "Animation (C010)";
        public override string Magic => MAGIC;

        public override int Size => 0x28;
        public override int ExtraSize => 0;

        private int Duration = 50;
        private int Unk1 = 0;
        private int Unk2 = 0;
        private float Unk3 = 0;
        private float Unk4 = 0;
        private string Path = "";
        private int Unk5 = 0;

        public C010() : base() { }

        public C010( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Duration = reader.ReadInt32();
            Unk1 = reader.ReadInt32();
            Unk2 = reader.ReadInt32();
            Unk3 = reader.ReadSingle();
            Unk4 = reader.ReadSingle();
            Path = reader.ReadOffsetString();
            Unk5 = reader.ReadInt32();
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            writer.Write( Duration );
            writer.Write( Unk1 );
            writer.Write( Unk2 );
            writer.Write( Unk3 );
            writer.Write( Unk4 );
            writer.WriteOffsetString( Path );
            writer.Write( Unk5 );
        }

        public override void Draw( string id ) {
            DrawHeader( id );
            ImGui.InputInt( $"Duration{id}", ref Duration );
            ImGui.InputInt( $"Unknown 1{id}", ref Unk1 );
            ImGui.InputInt( $"Unknown 2{id}", ref Unk2 );
            ImGui.InputFloat( $"Unknown 3{id}", ref Unk3 );
            ImGui.InputFloat( $"Unknown 4{id}", ref Unk4 );
            ImGui.InputText( $"Path{id}", ref Path, 31 );
            ImGui.InputInt( $"Unknown 5{id}", ref Unk5 );
        }
    }
}
