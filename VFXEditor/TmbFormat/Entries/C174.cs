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
            Parsed = new() {
                Unk1,
                Unk2,
                ScabbardPosition,
                Unk3,
                Unk4,
                Unk5,
                Unk6
            };

            Unk3.Value = 1;
            Unk4.Value = 1;
            ScabbardPosition.Value = 5;
        }

        public C174( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            ReadParsed( reader );
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            WriteParsed( writer );
        }

        public override void Draw( string id ) {
            DrawTime( id );
            DrawParsed( id );
        }
    }
}
