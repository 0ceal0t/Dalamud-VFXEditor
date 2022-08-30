using ImGuiNET;
using System.Numerics;
using VFXEditor.TmbFormat.Utils;

namespace VFXEditor.TmbFormat.Entries {
    public class C203 : TmbEntry {
        public const string MAGIC = "C203";
        public const string DISPLAY_NAME = "C203";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x2C;
        public override int ExtraSize => 0;

        private int Unk1 = 0;
        private int Unk2 = 0;
        private int Unk3 = 0;
        private int Unk4 = 0;
        private int Unk5 = 0;
        private int Unk6 = 0;
        private int Unk7 = 0;
        private float Unk8 = 0;

        public C203() : base() { }

        public C203( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Unk1 = reader.ReadInt32();
            Unk2 = reader.ReadInt32();
            Unk3 = reader.ReadInt32();
            Unk4 = reader.ReadInt32();
            Unk5 = reader.ReadInt32();
            Unk6 = reader.ReadInt32();
            Unk7 = reader.ReadInt32();
            Unk8 = reader.ReadSingle();
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            writer.Write( Unk1 );
            writer.Write( Unk2 );
            writer.Write( Unk3 );
            writer.Write( Unk4 );
            writer.Write( Unk5 );
            writer.Write( Unk6 );
            writer.Write( Unk7 );
            writer.Write( Unk8 );
        }

        public override void Draw( string id ) {
            DrawHeader( id );
            ImGui.InputInt( $"Unknown 1{id}", ref Unk1 );
            ImGui.InputInt( $"Unknown 2{id}", ref Unk2 );
            ImGui.InputInt( $"Unknown 3{id}", ref Unk3 );
            ImGui.InputInt( $"Unknown 4{id}", ref Unk4 );
            ImGui.InputInt( $"Unknown 5{id}", ref Unk5 );
            ImGui.InputInt( $"Unknown 6{id}", ref Unk6 );
            ImGui.InputInt( $"Unknown 7{id}", ref Unk7 );
            ImGui.InputFloat( $"Unknown 8{id}", ref Unk8 );
        }
    }
}
