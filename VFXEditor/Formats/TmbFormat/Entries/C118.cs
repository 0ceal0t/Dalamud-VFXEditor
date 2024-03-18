using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C118 : TmbEntry {
        public const string MAGIC = "C118";
        public const string DISPLAY_NAME = "Animation Transition";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x18;
        public override int ExtraSize => 0;

        private readonly ParsedInt TransitionTime = new( "Transition Time", value: 1 );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedInt Unk3 = new( "Unknown 3", value: 100 );

        public C118( TmbFile file ) : base( file ) { }

        public C118( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            TransitionTime,
            Unk2,
            Unk3
        ];
    }
}
