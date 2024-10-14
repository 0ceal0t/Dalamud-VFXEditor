using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C216 : TmbEntry {
        public const string MAGIC = "C216";
        public const string DISPLAY_NAME = "";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x30;
        public override int ExtraSize => 0;

        public readonly ParsedInt Unknown1 = new( "Unknown 1" );
        public readonly ParsedInt Unknown2 = new( "Unknown 2" );
        public readonly ParsedInt Unknown3 = new( "Unknown 3" );
        public readonly ParsedInt Unknown4 = new( "Unknown 4" );
        public readonly ParsedInt Unknown5 = new( "Unknown 5" );
        public readonly ParsedFloat Unknown6 = new( "Unknown 6" );
        public readonly ParsedInt Unknown7 = new( "Unknown 7" );
        public readonly ParsedInt Unknown8 = new( "Unknown 8" );
        public readonly ParsedInt Unknown9 = new( "Unknown 9" );

        public C216( TmbFile file ) : base( file ) { }

        public C216( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Unknown1,
            Unknown2,
            Unknown3,
            Unknown4,
            Unknown5,
            Unknown6,
            Unknown7,
            Unknown8,
            Unknown9
        ];
    }
}
