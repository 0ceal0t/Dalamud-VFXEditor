using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    // Moves character's server position while keeping client position
    // Don't mess with this

    public class C142 : TmbEntry {
        public const string MAGIC = "C142";
        public const string DISPLAY_NAME = "C142";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x1C;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedInt Unk3 = new( "Unknown 3" );
        private readonly ParsedInt Unk4 = new( "Unknown 4" );

        public C142( bool papEmbedded ) : base( papEmbedded ) { }

        public C142( TmbReader reader, bool papEmbedded ) : base( reader, papEmbedded ) {
            ReadHeader( reader );
            ReadParsed( reader );
        }

        protected override List<ParsedBase> GetParsed() => new() {
            Duration,
            Unk2,
            Unk3,
            Unk4
        };

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            WriteParsed( writer );
        }

        public override void Draw( string id ) {
            TmbFile.GenericWarning();

            DrawHeader( id );
            DrawParsed( id );
        }
    }
}
