using ImGuiNET;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C009 : TmbEntry {
        public const string MAGIC = "C009";
        public const string DISPLAY_NAME = "Animation - PAP Only (C009)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x18;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly TmbOffsetString Path = new( "Path", maxSize:31 );

        public C009() : base() {
            Parsed = new() {
                Duration,
                Unk1,
                Path
            };

            Duration.Value = 50;
        }

        public C009( TmbReader reader ) : base( reader ) {
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
