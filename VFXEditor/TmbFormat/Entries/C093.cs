using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C093 : TmbEntry {
        public const string MAGIC = "C093";
        public const string DISPLAY_NAME = "Color (C093)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x28;
        public override int ExtraSize => 4 * ( 4 + 4 );

        private readonly ParsedInt Duration = new( "Duration", defaultValue: 30 );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly TmbOffsetFloat4 Color1 = new( "Color 1", defaultValue: new( 1 ) );
        private readonly TmbOffsetFloat4 Color2 = new( "Color 2", defaultValue: new( 1 ) );
        private readonly ParsedInt Unk4 = new( "Unknown 4" );

        public C093( bool papEmbedded ) : base( papEmbedded ) { }

        public C093( TmbReader reader, bool papEmbedded ) : base( reader, papEmbedded ) {
            ReadHeader( reader );
            ReadParsed( reader );
        }

        protected override List<ParsedBase> GetParsed() => new() {
            Duration,
            Unk1,
            Color1,
            Color2,
            Unk4
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
