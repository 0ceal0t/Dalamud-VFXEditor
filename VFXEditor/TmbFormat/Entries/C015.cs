using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C015 : TmbEntry {
        public const string MAGIC = "C015";
        public const string DISPLAY_NAME = "Weapon Size (C015)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x1C;
        public override int ExtraSize => 0;

        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedInt WeaponSize = new( "Size" );
        private readonly ParsedEnum<ObjectControl> ObjectControl = new( "Object Control" );

        public C015( TmbFile file ) : base( file ) { }

        public C015( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => new() {
            Unk1,
            Unk2,
            WeaponSize,
            ObjectControl
        };
    }
}
