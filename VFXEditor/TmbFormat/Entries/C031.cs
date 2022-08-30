using ImGuiNET;
using VFXEditor.Utils;
using VFXEditor.TmbFormat.Utils;

namespace VFXEditor.TmbFormat.Entries {
    public class C031 : TmbEntry {
        public const string MAGIC = "C031";
        public const string DISPLAY_NAME = "Summon Animation (C031)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x18;
        public override int ExtraSize => 0;

        private int Duration = 0;
        private int Unk1 = 0;
        private short AnimationId = 0;
        private short TargetType = 2;

        public C031() : base() { }

        public C031( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Duration = reader.ReadInt32();
            Unk1 = reader.ReadInt32();
            AnimationId = reader.ReadInt16();
            TargetType = reader.ReadInt16();
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            writer.Write( Duration );
            writer.Write( Unk1 );
            writer.Write( AnimationId );
            writer.Write( TargetType );
        }

        public override void Draw( string id ) {
            DrawHeader( id );
            ImGui.InputInt( $"Duration{id}", ref Duration );
            ImGui.InputInt( $"Unknown 1{id}", ref Unk1 );
            FileUtils.ShortInput( $"Animation Id{id}", ref AnimationId );
            FileUtils.ShortInput( $"Target Type{id}", ref TargetType );
        }
    }
}
