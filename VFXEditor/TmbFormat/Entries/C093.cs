using ImGuiNET;
using System.Numerics;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C093 : TmbEntry {
        public const string MAGIC = "C093";
        public const string DISPLAY_NAME = "C093";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x28;
        public override int ExtraSize => 4 * ( 4 + 4 );

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly TmbOffsetFloat4 Unk2 = new( "Unknown 2" );
        private readonly TmbOffsetFloat4 Unk3 = new( "Unknown 3" );
        private readonly ParsedInt Unk4 = new( "Unknown 4" );

        public C093() : base() {
            Parsed = new() {
                Duration,
                Unk1,
                Unk2,
                Unk3
            };

            Duration.Value = 30;
            Unk2.Value = new( 1 );
            Unk3.Value = new( 1 );
        }

        public C093( TmbReader reader ) : base( reader ) {
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
