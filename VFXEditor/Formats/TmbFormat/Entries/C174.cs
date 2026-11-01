using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Base;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C174 : TmbEntry {
        public const string MAGIC = "C174";
        public const string DISPLAY_NAME = "Object Control";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x28;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedEnum<AtchState> InitialPosition = new( "Initial Position" );
        private readonly ParsedEnum<ObjectControl> ObjectControl = new( "Object Control" );
        private readonly ParsedEnum<ObjectControlFinal> FinalPosition = new( "Final Position" );
        private readonly ParsedBool PositionDelay = new( "Position Delay" );
        private readonly ParsedInt Unk6 = new( "Unknown 6" );

        public C174( TmbFile file ) : base( file ) { }

        public C174( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Duration,
            Unk2,
            InitialPosition,
            ObjectControl,
            FinalPosition,
            PositionDelay,
            Unk6
        ];
    }
}
