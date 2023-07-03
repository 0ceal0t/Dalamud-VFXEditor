using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C198 : TmbEntry {
        public const string MAGIC = "C198";
        public const string DISPLAY_NAME = "Lemure";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x28;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedInt Unk3 = new( "Unknown 3" );
        private readonly ParsedInt Unk4 = new( "Unknown 4" );
        private readonly ParsedShort ModelId = new( "Model Id" );
        private readonly ParsedShort BodyId = new( "Body Id" );
        private readonly ParsedInt Unk5 = new( "Unknown 5" );

        public C198( TmbFile file ) : base( file ) { }

        public C198( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => new() {
            Duration,
            Unk1,
            Unk2,
            Unk3,
            Unk4,
            ModelId,
            BodyId,
            Unk5
        };
    }
}
