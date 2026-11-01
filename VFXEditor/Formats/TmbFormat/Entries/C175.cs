using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Base;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C175 : TmbEntry {
        public const string MAGIC = "C175";
        public const string DISPLAY_NAME = "Object Scaling";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x28;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedEnum<AtchState> InitialScale = new( "Initial Scale" );
        private readonly ParsedEnum<ObjectControl> ObjectControl = new( "Object Control" );
        private readonly ParsedEnum<ObjectControlFinal> FinalScale = new( "Final Scale" );
        private readonly ParsedBool ScaleDelay = new( "Scale Delay" );
        private readonly ParsedInt Unk7 = new( "Unknown 7" );

        public C175( TmbFile file ) : base( file ) { }

        public C175( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Duration,
            Unk2,
            InitialScale,
            ObjectControl,
            FinalScale,
            ScaleDelay,
            Unk7
        ];
    }
}
