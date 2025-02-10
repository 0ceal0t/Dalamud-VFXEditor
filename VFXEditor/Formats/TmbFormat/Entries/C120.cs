using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C120 : TmbEntry {
        public const string MAGIC = "C120";
        public const string DISPLAY_NAME = "Controller Vibration";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x18;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration", value: 1 );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedInt WaveType = new( "Wave Type", value: 100 );

        public C120( TmbFile file ) : base( file ) { }

        public C120( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Duration,
            Unk2,
            WaveType
        ];
    }
}
