using System.Collections.Generic;
using VfxEditor.Formats.TmbFormat.Base;
using VfxEditor.Formats.TmbFormat.Utils;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Enums;
using VfxEditor.Parsing.Int;

namespace VfxEditor.Formats.TmbFormat.Entries {
    public class C202 : TmbEntry {
        public const string MAGIC = "C202";
        public const string DISPLAY_NAME = "Weapon Part Visibility";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x24;
        public override int ExtraSize => 0;

        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedEnum<C202WeaponPart> Part = new( "Weapon Part" );
        private readonly ParsedByte HideAtStart = new( "Hide At Start" );
        private readonly ParsedByte HideAtEnd = new( "Hide At End" );
        private readonly ParsedShort Unk4 = new( "Unknown 4" );
        private readonly ParsedInt Unk5 = new( "Unknown 5" );
        private readonly ParsedInt Unk6 = new( "Unknown 6" );

        public C202( TmbFile file ) : base( file ) { }

        public C202( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Unk1,
            Unk2,
            Part,
            HideAtStart,
            HideAtEnd,
            Unk4,
            Unk5,
            Unk6
        ];
    }
}