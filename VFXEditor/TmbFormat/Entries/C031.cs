using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Sheets;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C031 : TmbEntry {
        public const string MAGIC = "C031";
        public const string DISPLAY_NAME = "Summon Animation (C031)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x18;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedWeaponTimeline Animation = new( "Animation" );
        private readonly ParsedEnum<ObjectControl> TargetType = new( "Target Type", size: 2 );

        public C031( TmbFile file ) : base( file ) { }

        public C031( TmbFile file, TmbReader reader ) : base( file, reader ) {
            ReadHeader( reader );
            ReadParsed( reader );
        }

        protected override List<ParsedBase> GetParsed() => new() {
            Duration,
            Unk1,
            Animation,
            TargetType
        };
    }
}
