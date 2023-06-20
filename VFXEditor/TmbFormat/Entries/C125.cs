using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C125 : TmbEntry {
        public const string MAGIC = "C125";
        public const string DISPLAY_NAME = "Animation Lock (C125)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;
        public override DangerLevel Danger => DangerLevel.Yellow;

        public override int Size => 0x14;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration", defaultValue: 1 );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );

        public C125( TmbFile file ) : base( file ) { }

        public C125( TmbFile file, TmbReader reader ) : base( file, reader ) {
            ReadHeader( reader );
            ReadParsed( reader );
        }

        protected override List<ParsedBase> GetParsed() => new() {
            Duration,
            Unk1
        };
    }
}
