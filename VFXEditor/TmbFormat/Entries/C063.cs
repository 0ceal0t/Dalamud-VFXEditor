using ImGuiNET;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C063 : TmbEntry {
        public const string MAGIC = "C063";
        public const string DISPLAY_NAME = "Sound (C063)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x20;
        public override int ExtraSize => 0;

        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private string Path = "";
        private readonly ParsedInt SoundIndex = new( "Sound Index" );
        private readonly ParsedInt Unk3 = new( "Unknown 3" );

        public C063() : base() {
            Unk1.Value = 1;
            SoundIndex.Value = 1;
        }

        public C063( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Unk1.Read( reader );
            Unk2.Read( reader );
            Path = reader.ReadOffsetString();
            SoundIndex.Read( reader );
            Unk3.Read( reader );
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            Unk1.Write( writer );
            Unk2.Write( writer );
            writer.WriteOffsetString( Path );
            SoundIndex.Write( writer );
            Unk3.Write( writer );
        }

        public override void Draw( string id ) {
            DrawTime( id );
            Unk1.Draw( id, CommandManager.Tmb );
            Unk2.Draw( id, CommandManager.Tmb );
            ImGui.InputText( $"Path{id}", ref Path, 255 );
            SoundIndex.Draw( id, CommandManager.Tmb );
            Unk3.Draw( id, CommandManager.Tmb );
        }
    }
}
