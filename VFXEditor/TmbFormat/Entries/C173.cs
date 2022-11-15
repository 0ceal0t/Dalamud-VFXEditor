using ImGuiNET;
using System.Numerics;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Parsing;

namespace VfxEditor.TmbFormat.Entries {
    public class C173 : TmbEntry {
        public const string MAGIC = "C173";
        public const string DISPLAY_NAME = "VFX (C173)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x44;
        public override int ExtraSize => 0;

        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private string Path = "";
        private readonly ParsedShort BindPoint1 = new( "Bind Point 1" );
        private readonly ParsedShort BindPoint2 = new( "Bind Point 2" );
        private readonly ParsedInt Unk3 = new( "Unknown 3" );
        private readonly ParsedInt Unk4 = new( "Unknown 4" );
        private readonly ParsedInt Unk5 = new( "Unknown 5" );
        private readonly ParsedInt Unk6 = new( "Unknown 6" );
        private readonly ParsedInt Unk7 = new( "Unknown 7" );
        private readonly ParsedInt Unk8 = new( "Unknown 8" );
        private readonly ParsedInt Unk9 = new( "Unknown 9" );
        private readonly ParsedInt Unk10 = new( "Unknown 10" );
        private readonly ParsedInt Unk11 = new( "Unknown 11" );
        private readonly ParsedInt Unk12 = new( "Unknown 12" );

        public C173() : base() {
            Unk1.Value = 30;
            BindPoint1.Value = 1;
            BindPoint2.Value = 0xFF;
        }

        public C173( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Unk1.Read( reader );
            Unk2.Read( reader );
            Path = reader.ReadOffsetString();
            BindPoint1.Read( reader );
            BindPoint2.Read( reader );
            Unk3.Read( reader );
            Unk4.Read( reader );
            Unk5.Read( reader );
            Unk6.Read( reader );
            Unk7.Read( reader );
            Unk8.Read( reader );
            Unk9.Read( reader );
            Unk10.Read( reader );
            Unk11.Read( reader );
            Unk12.Read( reader );
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            Unk1.Write( writer );
            Unk2.Write( writer );
            writer.WriteOffsetString( Path );
            BindPoint1.Write( writer );
            BindPoint2.Write( writer );
            Unk3.Write( writer );
            Unk4.Write( writer );
            Unk5.Write( writer );
            Unk6.Write( writer );
            Unk7.Write( writer );
            Unk8.Write( writer );
            Unk9.Write( writer );
            Unk10.Write( writer );
            Unk11.Write( writer );
            Unk12.Write( writer );
        }

        public override void Draw( string id ) {
            DrawTime( id );
            Unk1.Draw( id, CommandManager.Tmb );
            Unk2.Draw( id, CommandManager.Tmb );
            ImGui.InputText( $"Path{id}", ref Path, 255 );
            BindPoint1.Draw( id, CommandManager.Tmb );
            BindPoint2.Draw( id, CommandManager.Tmb );
            Unk3.Draw( id, CommandManager.Tmb );
            Unk4.Draw( id, CommandManager.Tmb );
            Unk5.Draw( id, CommandManager.Tmb );
            Unk6.Draw( id, CommandManager.Tmb );
            Unk7.Draw( id, CommandManager.Tmb );
            Unk8.Draw( id, CommandManager.Tmb );
            Unk9.Draw( id, CommandManager.Tmb );
            Unk10.Draw( id, CommandManager.Tmb );
            Unk11.Draw( id, CommandManager.Tmb );
            Unk12.Draw( id, CommandManager.Tmb );
        }
    }
}
