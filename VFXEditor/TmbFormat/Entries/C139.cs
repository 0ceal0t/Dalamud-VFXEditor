using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C139 : TmbEntry {
        public const string MAGIC = "C139";
        public const string DISPLAY_NAME = "";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;
        public override DangerLevel Danger => DangerLevel.Yellow;

        public override int Size => 0x14;
        public override int ExtraSize => 0;

        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );

        public C139( TmbFile file ) : base( file ) { }

        public C139( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => new() {
            Unk1,
            Unk2
        };
    }
}
