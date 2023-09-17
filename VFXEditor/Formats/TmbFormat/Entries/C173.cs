using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C173 : TmbEntry {
        public const string MAGIC = "C173";
        public const string DISPLAY_NAME = "VFX";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x44;
        public override int ExtraSize => 0;

        private readonly ParsedInt Unk1 = new( "Unknown 1", value: 1 );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly TmbOffsetString Path = new( "Path" );
        private readonly ParsedShort BindPoint1 = new( "Bind Point 1", value: 1 );
        private readonly ParsedShort BindPoint2 = new( "Bind Point 2", value: 0xFF );
        private readonly ParsedInt Unk3 = new( "Unknown 3" );
        private readonly ParsedInt Unk4 = new( "Unknown 4" );
        private readonly ParsedInt Unk5 = new( "Unknown 5" );
        private readonly ParsedInt Unk6 = new( "Unknown 6" );
        private readonly ParsedInt Unk7 = new( "Unknown 7" );
        private readonly ParsedInt Unk8 = new( "Unknown 8" );
        private readonly ParsedInt Unk9 = new( "Unknown 9" );
        private readonly ParsedInt Unk10 = new( "Unknown 10" );
        private readonly ParsedInt Unk11 = new( "Unknown 11" );
        private readonly ParsedInt Unk12 = new( "Unknown 12" );

        public C173( TmbFile file ) : base( file ) { }

        public C173( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => new() {
            Unk1,
            Unk2,
            Path,
            BindPoint1,
            BindPoint2,
            Unk3,
            Unk4,
            Unk5,
            Unk6,
            Unk7,
            Unk8,
            Unk9,
            Unk10,
            Unk11,
            Unk12
        };
    }
}
