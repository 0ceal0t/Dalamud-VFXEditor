using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C139 : TmbEntry {
        public const string MAGIC = "C139";
        public const string DISPLAY_NAME = "Forced Movement Cancel";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;
        public override DangerLevel Danger => DangerLevel.Detectable;

        public override int Size => 0x14;
        public override int ExtraSize => 0;

        private readonly ParsedBool Enabled = new( "Enabled" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );

        public C139( TmbFile file ) : base( file ) { }

        public C139( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Enabled,
            Unk2
        ];
    }
}
