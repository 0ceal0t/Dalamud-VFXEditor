using ImGuiNET;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C010 : TmbEntry {
        public const string MAGIC = "C010";
        public const string DISPLAY_NAME = "Animation (C010)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x28;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedFloat Unk3 = new( "Unknown 3" );
        private readonly ParsedFloat Unk4 = new( "Unknown 4" );
        private string Path = "";
        private readonly ParsedInt Unk5 = new( "Unknown 1" );

        public C010() : base() {
            Duration.Value = 50;
        }

        public C010( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Duration.Read( reader );
            Unk1.Read( reader );
            Unk2.Read( reader );
            Unk3.Read( reader );
            Unk4.Read( reader );
            Path = reader.ReadOffsetString();
            Unk5.Read( reader );
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            Duration.Write( writer );
            Unk1.Write( writer );
            Unk2.Write( writer );
            Unk3.Write( writer );
            Unk4.Write( writer );
            writer.WriteOffsetString( Path );
            Unk5.Write( writer );
        }

        public override void Draw( string id ) {
            DrawTime( id );
            Duration.Draw( id, CommandManager.Tmb );
            Unk1.Draw( id, CommandManager.Tmb );
            Unk2.Draw( id, CommandManager.Tmb );
            Unk3.Draw( id, CommandManager.Tmb );
            Unk4.Draw( id, CommandManager.Tmb );
            ImGui.InputText( $"Path{id}", ref Path, 31 );
            Unk5.Draw( id, CommandManager.Tmb );
        }
    }
}
