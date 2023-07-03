using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C014 : TmbEntry {
        public const string MAGIC = "C014";
        public const string DISPLAY_NAME = "Weapon Position";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x1C;
        public override int ExtraSize => 0;

        private readonly ParsedBool Enabled = new( "Enabled" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedEnum<ObjectControlPosition> ObjectPosition = new( "Object Position" );
        private readonly ParsedEnum<ObjectControl> ObjectControl = new( "Object Control" );

        public C014( TmbFile file ) : base( file ) { }

        public C014( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => new() {
            Enabled,
            Unk2,
            ObjectPosition,
            ObjectControl
        };
    }
}
