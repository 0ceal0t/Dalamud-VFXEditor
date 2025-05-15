using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C107 : TmbEntry {
        public const string MAGIC = "C107";
        public const string DISPLAY_NAME = "VFX Trigger";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x1C;
        public override int ExtraSize => 0;

        private readonly ParsedInt Enabled = new( "Enabled" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedInt TriggerRow = new( "Row" ); //row + 1
        private readonly ParsedInt Unk4 = new( "Unknown 4" );

        public C107( TmbFile file ) : base( file ) { }

        public C107( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Enabled,
            Unk2,
            TriggerRow,
            Unk4
        ];
    }
}
