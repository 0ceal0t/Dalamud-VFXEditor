using ImGuiNET;
using System.Numerics;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C131 : TmbEntry {
        public const string MAGIC = "C131";
        public const string DISPLAY_NAME = "Animation Cancelled by Movement (C131)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x14;
        public override int ExtraSize => 0;

        private int Unk1 = 0;
        private int Unk2 = 0;

        public C131() : base() { }

        public C131( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Unk1 = reader.ReadInt32();
            Unk2 = reader.ReadInt32();
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            writer.Write( Unk1 );
            writer.Write( Unk2 );
        }

        public override void Draw( string id ) {
            DrawHeader( id );
            ImGui.InputInt( $"Unknown 1{id}", ref Unk1 );
            ImGui.InputInt( $"Unknown 1{id}", ref Unk2 );
        }
    }
}
