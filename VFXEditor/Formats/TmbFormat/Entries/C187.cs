using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Base;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C187 : TmbEntry {
        public const string MAGIC = "C187";
        public const string DISPLAY_NAME = "Remove Part";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x20;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedShort Part = new( "Part" );
        private readonly ParsedBool Inverse = new( "Inverse", size: 2 );
        private readonly ParsedEnum<C187ObjectControl> ObjectControl = new( "Object Control", size: 2 );
        private readonly ParsedByteBool Unk2 = new( "Unknown 2" );
        private readonly ParsedByteBool NoDelay = new( "No Delay" );
        private readonly ParsedInt Unk3 = new( "Unknown 3" );

        public C187( TmbFile file ) : base( file ) { }

        public C187( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Duration,
            Unk1,
            Part,
            Inverse,
            ObjectControl,
            Unk2,
            NoDelay,
            Unk3
        ];
    }
}
