using ImGuiNET;
using System.Numerics;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C187 : TmbEntry {
        public const string MAGIC = "C187";
        public const string DISPLAY_NAME = "C187";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x20;
        public override int ExtraSize => 0;

        private int Unk1 = 0;
        private int Unk2 = 0;
        private int Unk3 = 4;
        private int Unk4 = 0;
        private int Unk5 = 1;

        public C187() : base() { }

        public C187( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Unk1 = reader.ReadInt32();
            Unk2 = reader.ReadInt32();
            Unk3 = reader.ReadInt32();
            Unk4 = reader.ReadInt32();
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
            ImGui.InputInt( $"Unknown 3{id}", ref Unk3 );
            ImGui.InputInt( $"Unknown 4{id}", ref Unk4 );
            ImGui.InputInt( $"Unknown 5{id}", ref Unk5 );
        }
    }
}
