using System.Collections.Generic;
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

        private readonly ParsedInt Loop = new( "Loop", defaultValue: 1 );
        private readonly ParsedInt Interrupt = new( "Interrupt" );
        private readonly TmbOffsetString Path = new( "Path" );
        private readonly ParsedInt SoundIndex = new( "Sound Index" );
        private readonly ParsedInt SoundPosition = new( "Sound Position", defaultValue: 1 );

        public C063( bool papEmbedded ) : base( papEmbedded ) { }

        public C063( TmbReader reader, bool papEmbedded ) : base( reader, papEmbedded ) {
            ReadHeader( reader );
            ReadParsed( reader );
        }

        protected override List<ParsedBase> GetParsed() => new() {
            Loop,
            Interrupt,
            Path,
            SoundIndex,
            SoundPosition
        };
    }
}
