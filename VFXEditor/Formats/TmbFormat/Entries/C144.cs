using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C144 : TmbEntry {
        public const string MAGIC = "C144";
        public const string DISPLAY_NAME = "Camera and Nameplate Control";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x2C;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedInt Unk3 = new( "Unknown 3" );
        private readonly ParsedFloat2 Camera = new( "Camera Shift" );
        private readonly ParsedFloat3 Nameplate = new( "Nameplate Shift" );

        public C144( TmbFile file ) : base( file ) { }

        public C144( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => new() {
            Duration,
            Unk2,
            Unk3,
            Camera,
            Nameplate
        };
    }
}
