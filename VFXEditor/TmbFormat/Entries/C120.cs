using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C120 : TmbEntry {
        public const string MAGIC = "C120";
        public const string DISPLAY_NAME = "C120";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x18;
        public override int ExtraSize => 0;

        private readonly ParsedInt Unk1 = new( "Unknown 1", defaultValue: 1 );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedInt Unk3 = new( "Unknown 3", defaultValue: 100 );

        public C120( TmbFile file ) : base( file ) { }

        public C120( TmbFile file, TmbReader reader ) : base( file, reader ) {
            ReadHeader( reader );
            ReadParsed( reader );
        }

        protected override List<ParsedBase> GetParsed() => new() {
            Unk1,
            Unk2,
            Unk3
        };
    }
}
