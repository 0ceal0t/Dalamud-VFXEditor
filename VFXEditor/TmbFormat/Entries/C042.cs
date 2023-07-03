using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C042 : TmbEntry {
        public const string MAGIC = "C042";
        public const string DISPLAY_NAME = "Footstep";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x1C;
        public override int ExtraSize => 0;

        private readonly ParsedBool Enabled = new( "Enabled" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedInt FootId = new( "Bind Id" );
        private readonly ParsedInt SoundId = new( "Sound Id" );

        public C042( TmbFile file ) : base( file ) { }

        public C042( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => new() {
            Enabled,
            Unk2,
            FootId,
            SoundId
        };
    }
}
