using System.Collections.Generic;
using VfxEditor.Formats.TmbFormat.Utils;
using VfxEditor.Parsing;

namespace VfxEditor.Formats.TmbFormat.Entries {
    public class C100 : TmbEntry {
        public const string MAGIC = "C100";
        public const string DISPLAY_NAME = "Hide Weapon";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x20;
        public override int ExtraSize => 0;

        private readonly ParsedBool Enabled = new( "Enabled" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedShort Visiblity = new( "Visibility" );
        private readonly ParsedShort Unk3 = new( "Unknown 3" );
        private readonly ParsedInt Unk4 = new( "Unknown 4" );
        private readonly ParsedInt Unk5 = new( "Unknown 5" );

        public C100( TmbFile file ) : base( file ) { }

        public C100( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Enabled,
            Unk2,
            Visiblity,
            Unk3,
            Unk4,
            Unk5
        ];
    }
}
