using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C187 : TmbEntry {
        public const string MAGIC = "C187";
        public const string DISPLAY_NAME = "C187";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x20;
        public override int ExtraSize => 0;

        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedInt Unk3 = new( "Unknown 3", defaultValue: 4 );
        private readonly ParsedInt Unk4 = new( "Unknown 4" );
        private readonly ParsedInt Unk5 = new( "Unknown 5", defaultValue: 1 );

        public C187( bool papEmbedded ) : base( papEmbedded ) { }

        public C187( TmbReader reader, bool papEmbedded ) : base( reader, papEmbedded ) {
            ReadHeader( reader );
            ReadParsed( reader );
        }

        protected override List<ParsedBase> GetParsed() => new() {
            Unk1,
            Unk2,
            Unk3,
            Unk4,
            Unk5
        };

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            WriteParsed( writer );
        }

        public override void Draw( string id ) {
            DrawHeader( id );
            DrawParsed( id );
        }
    }
}
