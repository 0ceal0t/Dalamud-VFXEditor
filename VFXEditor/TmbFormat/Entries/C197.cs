using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C197 : TmbEntry {
        public const string MAGIC = "C197";
        public const string DISPLAY_NAME = "Voiceline (C197)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x24;
        public override int ExtraSize => 0;

        private readonly ParsedInt FadeTime = new( "Fade Time" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedInt VoicelineNumber = new( "Voiceline Number" );
        private readonly ParsedInt BindPointId = new( "Bind Point ID" );
        private readonly ParsedInt Unk5 = new( "Unknown 5" );
        private readonly ParsedInt Unk6 = new( "Unknown 6" );

        public C197( TmbFile file ) : base( file ) { }

        public C197( TmbFile file, TmbReader reader ) : base( file, reader ) {
            ReadHeader( reader );
            ReadParsed( reader );
        }

        protected override List<ParsedBase> GetParsed() => new() {
            FadeTime,
            Unk2,
            VoicelineNumber,
            BindPointId,
            Unk5,
            Unk6
        };
    }
}
