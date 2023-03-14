using ImGuiNET;
using System.Numerics;
using VfxEditor.Utils;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Parsing;
using System.Collections.Generic;

namespace VfxEditor.TmbFormat.Entries {
    public class C012 : TmbEntry {
        public const string MAGIC = "C012";
        public const string DISPLAY_NAME = "VFX (C012)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x48;
        public override int ExtraSize => 4 * ( 3 + 3 + 3 + 4 );

        private readonly ParsedInt Duration = new( "Duration", defaultValue: 30 );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly TmbOffsetString Path = new( "Path" );
        private readonly ParsedShort BindPoint1 = new( "Bind Point 1", defaultValue: 1 );
        private readonly ParsedShort BindPoint2 = new( "Bind Point 2", defaultValue: 0xFF );
        private readonly ParsedShort BindPoint3 = new( "Bind Point 3", defaultValue: 2 );
        private readonly ParsedShort BindPoint4 = new( "Bind Point 4", defaultValue: 0xFF );
        private readonly TmbOffsetFloat3 Scale = new( "Scale", defaultValue: new( 1 ) );
        private readonly TmbOffsetFloat3 Rotation = new( "Rotation" );
        private readonly TmbOffsetFloat3 Position = new( "Position" );
        private readonly TmbOffsetFloat4 RGBA = new( "RGBA", defaultValue: new( 1 ) );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedInt Unk3 = new( "Unknown 3" );

        public C012( bool papEmbedded ) : base( papEmbedded ) { }

        public C012( TmbReader reader, bool papEmbedded ) : base( reader, papEmbedded ) {
            ReadHeader( reader );
            ReadParsed( reader );
        }

        protected override List<ParsedBase> GetParsed() => new() {
            Duration,
            Unk1,
            Path,
            BindPoint1,
            BindPoint2,
            BindPoint3,
            BindPoint4,
            Scale,
            Rotation,
            Position,
            RGBA,
            Unk2,
            Unk3
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
