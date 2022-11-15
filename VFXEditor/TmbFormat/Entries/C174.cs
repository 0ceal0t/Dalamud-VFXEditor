using ImGuiNET;
using System.Numerics;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C174 : TmbEntry {
        public const string MAGIC = "C174";
        public const string DISPLAY_NAME = "Scabbard (C174)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x28;
        public override int ExtraSize => 0;

        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedInt ScabbardPosition = new( "Scabbard Position" );
        private readonly ParsedInt Unk3 = new( "Unknown 3" );
        private readonly ParsedInt Unk4 = new( "Unknown 4" );
        private readonly ParsedInt Unk5 = new( "Unknown 5" );
        private readonly ParsedInt Unk6 = new( "Unknown 6" );

        public C174() : base() {
            Unk3.Value = 1;
            Unk4.Value = 1;
            ScabbardPosition.Value = 5;
        }

        public C174( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            Unk1.Read( reader );
            Unk2.Read( reader );
            ScabbardPosition.Read( reader );
            Unk3.Read( reader );
            Unk4.Read( reader );
            Unk5.Read( reader );
            Unk6.Read( reader );
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            Unk1.Write( writer );
            Unk2.Write( writer );
            ScabbardPosition.Write( writer );
            Unk3.Write( writer );
            Unk4.Write( writer );
            Unk5.Write( writer );
            Unk6.Write( writer );
        }

        public override void Draw( string id ) {
            DrawTime( id );
            Unk1.Draw( id, CommandManager.Tmb );
            Unk2.Draw( id, CommandManager.Tmb );
            ScabbardPosition.Draw( id, CommandManager.Tmb );
            Unk3.Draw( id, CommandManager.Tmb );
            Unk4.Draw( id, CommandManager.Tmb );
            Unk5.Draw( id, CommandManager.Tmb );
            Unk6.Draw( id, CommandManager.Tmb );
        }
    }
}
