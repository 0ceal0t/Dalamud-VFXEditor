using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public enum AtchState {
        State_1 = 0,
        State_0 = 1,
        State_2 = 2,
        State_3 = 3,
        State_4 = 4,
        State_5 = 5,
    }
    public class C015 : TmbEntry {
        public const string MAGIC = "C015";
        public const string DISPLAY_NAME = "Weapon Size";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x1C;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedEnum<AtchState> WeaponSize = new( "ATCH Object Scale" );
        private readonly ParsedEnum<ObjectControl> ObjectControl = new( "Object Control" );

        public C015( TmbFile file ) : base( file ) { }

        public C015( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Duration,
            Unk2,
            WeaponSize,
            ObjectControl
        ];
    }
}
