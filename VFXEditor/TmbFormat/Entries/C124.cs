using ImGuiNET;
using System.Numerics;
using VFXEditor.TmbFormat.Utils;

namespace VFXEditor.TmbFormat.Entries {
    public class C124 : TmbEntry {
        public const string MAGIC = "C124";
        public const string DISPLAY_NAME = "C124";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x18;
        public override int ExtraSize => 0;

        private int Unk1 = 1;
        private int Unk2 = 0;
        private int Unk3 = 100;

        public C124() : base() { }

        public C124( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Unk1 = reader.ReadInt32();
            Unk2 = reader.ReadInt32();
            Unk3 = reader.ReadInt32();
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            writer.Write( Unk1 );
            writer.Write( Unk2 );
            writer.Write( Unk3 );
        }

        public override void Draw( string id ) {
            DrawHeader( id );
            ImGui.InputInt( $"Unknown 1{id}", ref Unk1 );
            ImGui.InputInt( $"Unknown 2{id}", ref Unk2 );
            ImGui.InputInt( $"Unknown 3{id}", ref Unk3 );
        }
    }
}
