using ImGuiNET;
using VfxEditor.Utils;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Parsing;

namespace VfxEditor.TmbFormat.Entries {
    public class C031 : TmbEntry {
        public const string MAGIC = "C031";
        public const string DISPLAY_NAME = "Summon Animation (C031)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x18;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedShort AnimationId = new( "Animation Id" );
        private readonly ParsedShort TargetType = new( "Target Type" );

        public C031() : base() {
            Parsed = new() {
                Duration,
                Unk1,
                AnimationId,
                TargetType
            };

            TargetType.Value = 2;
        }

        public C031( TmbReader reader ) : base( reader ) {
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
