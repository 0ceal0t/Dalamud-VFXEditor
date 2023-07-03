using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C068 : TmbEntry {
        public const string MAGIC = "C068";
        public const string DISPLAY_NAME = "Shade Black";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x24;
        public override int ExtraSize => 0;

        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedInt Unk3 = new( "Unknown 3" );
        private readonly ParsedInt Unk4 = new( "Unknown 4" );
        private readonly ParsedInt Unk5 = new( "Unknown 5" );
        private readonly ParsedInt Unk6 = new( "Unknown 6" );

        public C068( TmbFile file ) : base( file ) { }

        public C068( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => new() {
            Unk1,
            Unk2,
            Unk3,
            Unk4,
            Unk5,
            Unk6
        };
    }
}
