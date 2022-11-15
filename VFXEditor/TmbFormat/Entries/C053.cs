using ImGuiNET;
using VfxEditor.Utils;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Parsing;

namespace VfxEditor.TmbFormat.Entries {
    public class C053 : TmbEntry {
        public const string MAGIC = "C053";
        public const string DISPLAY_NAME = "Voiceline (C053)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x1C;
        public override int ExtraSize => 0;

        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedShort SoundId1 = new( "Sound Id 1" );
        private readonly ParsedShort SoundId2 = new( "Sound Id 2" );
        private readonly ParsedInt Unk3 = new( "Unknown 3" );

        public C053() : base() {
            Parsed = new() {
                Unk1,
                Unk2,
                SoundId1,
                SoundId2,
                Unk3
            };
        }

        public C053( TmbReader reader ) : base( reader ) {
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
