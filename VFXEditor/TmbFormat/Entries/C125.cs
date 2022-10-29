using ImGuiNET;
using System.Numerics;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C125 : TmbEntry {
        public const string MAGIC = "C125";
        public const string DISPLAY_NAME = "Animation Lock (C125)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x14;
        public override int ExtraSize => 0;

        private int Duration = 1;
        private int Unk1 = 0;

        public C125() : base() { }

        public C125( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Duration = reader.ReadInt32();
            Unk1 = reader.ReadInt32();
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            writer.Write( Duration );
            writer.Write( Unk1 );
        }

        public override void Draw( string id ) {
            ImGui.TextColored( new Vector4( 1, 0, 0, 1 ), "Please don't do anything stupid with this" );

            DrawHeader( id );
            ImGui.InputInt( $"Duration{id}", ref Duration );
            ImGui.InputInt( $"Unknown 1{id}", ref Unk1 );
        }
    }
}
