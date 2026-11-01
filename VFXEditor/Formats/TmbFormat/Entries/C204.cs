using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C204 : TmbEntry {
        public const string MAGIC = "C204";
        public const string DISPLAY_NAME = "Shroud Transform"; //applies to both RPR and SCH
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x1C;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedBool Hide = new( "Hide Transform" );
        private readonly ParsedInt Unk4 = new( "Unknown 4" );

        public C204( TmbFile file ) : base( file ) { }

        public C204( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Duration,
            Unk2,
            Hide,
            Unk4
        ];
    }
}
