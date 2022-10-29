using ImGuiNET;
using VfxEditor.Utils;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C053 : TmbEntry {
        public const string MAGIC = "C053";
        public const string DISPLAY_NAME = "Voiceline (C053)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x1C;
        public override int ExtraSize => 0;

        private int Unk1 = 0;
        private int Unk2 = 0;
        private short SoundId1 = 0;
        private short SoundId2 = 0;
        private int Unk3 = 0;

        public C053() : base() { }

        public C053( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Unk1 = reader.ReadInt32();
            Unk2 = reader.ReadInt32();
            SoundId1 = reader.ReadInt16();
            SoundId2 = reader.ReadInt16();
            Unk3 = reader.ReadInt32();
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            writer.Write( Unk1 );
            writer.Write( Unk2 );
            writer.Write( SoundId1 );
            writer.Write( SoundId2 );
            writer.Write( Unk3 );
        }

        public override void Draw( string id ) {
            DrawHeader( id );
            ImGui.InputInt( $"Unknown 1{id}", ref Unk1 );
            ImGui.InputInt( $"Unknown 2{id}", ref Unk2 );
            FileUtils.ShortInput( $"Sound Id 1{id}", ref SoundId1 );
            FileUtils.ShortInput( $"Sound Id 2{id}", ref SoundId2 );
            ImGui.InputInt( $"Unknown 3{id}", ref Unk3 );
        }
    }
}
