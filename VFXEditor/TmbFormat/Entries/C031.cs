using ImGuiNET;
using VFXEditor.Helper;
using VFXEditor.TmbFormat.Utils;

namespace VFXEditor.TmbFormat.Entries {
    public class C031 : TmbEntry {
        public const string MAGIC = "C031";
        public override string DisplayName => "Summon Animation (C031)";
        public override string Magic => MAGIC;

        public override int Size => 0x18;
        public override int ExtraSize => 0;

        private int Duration = 0;
        private int Unk1 = 0;
        private short AnimationId = 0;
        private short Unk2 = 2;

        public C031() : base() { }

        public C031( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Duration = reader.ReadInt32();
            Unk1 = reader.ReadInt32();
            AnimationId = reader.ReadInt16();
            Unk2 = reader.ReadInt16();
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            writer.Write( Duration );
            writer.Write( Unk1 );
            writer.Write( AnimationId );
            writer.Write( Unk2 );
        }

        public override void Draw( string id ) {
            DrawHeader( id );
            ImGui.InputInt( $"Duration{id}", ref Duration );
            ImGui.InputInt( $"Unknown 1{id}", ref Unk1 );
            FileHelper.ShortInput( $"Animation Id{id}", ref AnimationId );
            FileHelper.ShortInput( $"Unknown 2{id}", ref Unk2 );
        }
    }
}
