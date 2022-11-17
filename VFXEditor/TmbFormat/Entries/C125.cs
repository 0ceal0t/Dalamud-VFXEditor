using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C125 : TmbEntry {
        public const string MAGIC = "C125";
        public const string DISPLAY_NAME = "Animation Lock (C125)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x14;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration", defaultValue: 1 );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );

        public C125( bool papEmbedded ) : base( papEmbedded ) { }

        public C125( TmbReader reader, bool papEmbedded ) : base( reader, papEmbedded ) {
            ReadHeader( reader );
            ReadParsed( reader );
        }

        protected override List<ParsedBase> GetParsed() => new() {
            Duration,
            Unk1
        };

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            WriteParsed( writer );
        }

        public override void Draw( string id ) {
            ImGui.TextColored( new Vector4( 1, 0, 0, 1 ), "Please don't do anything stupid with this" );

            DrawTime( id );
            DrawParsed( id );
        }
    }
}
