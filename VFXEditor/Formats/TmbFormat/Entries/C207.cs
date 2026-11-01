using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C207 : TmbEntry {
        public const string MAGIC = "C207";
        public const string DISPLAY_NAME = "Lightning";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x2C;
        public override int ExtraSize => 4 * (4 + 3);

        private readonly ParsedBool Enabled = new( "Enabled" );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly TmbOffsetFloat4 RGBA = new( "Color" );
        private readonly TmbOffsetFloat3 Loc = new( "Translation" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" ); //0
        private readonly ParsedInt Unk3 = new( "Unknown 3" ); //0


        public C207( TmbFile file ) : base( file ) { }

        public C207( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Enabled,
            Unk1,
            RGBA,
            Loc,
            Unk2,
            Unk3
        ];
    }
}
