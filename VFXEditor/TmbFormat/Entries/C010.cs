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
        private readonly TmbOffsetString Path = new( "Path", maxSize:31 );
        private readonly ParsedInt Unk5 = new( "Unknown 1" );

        public C010() : base() {
            Parsed = new() {
                Duration,
                Unk1,
                Unk2,
                Unk3,
                Unk4,
                Path,
                Unk5
            };

            Duration.Value = 50;
        }

        public C010( TmbReader reader ) : base( reader ) {
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
