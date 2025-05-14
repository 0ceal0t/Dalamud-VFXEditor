using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C161 : TmbEntry {
        public const string MAGIC = "C161";
        public const string DISPLAY_NAME = "Blink";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x1C;
        public override int ExtraSize => 0;

        private readonly ParsedInt Enabled = new( "Enabled" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedBool Blink = new( "Blink" );
        private readonly ParsedInt Unk4 = new( "Unknown 4" );

        public C161( TmbFile file ) : base( file ) { }

        public C161( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Enabled,
            Unk2,
            Blink,
            Unk4
        ];
    }
}
