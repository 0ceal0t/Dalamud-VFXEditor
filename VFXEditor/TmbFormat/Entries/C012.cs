using ImGuiNET;
using System.Numerics;
using VfxEditor.Utils;
using VfxEditor.TmbFormat.Utils;
using VfxEditor.Parsing;

namespace VfxEditor.TmbFormat.Entries {
    public class C012 : TmbEntry {
        public const string MAGIC = "C012";
        public const string DISPLAY_NAME = "VFX (C012)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x48;
        public override int ExtraSize => 4 * ( 3 + 3 + 3 + 4 );

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly TmbOffsetString Path = new( "Path" );
        private readonly ParsedShort BindPoint1 = new( "Bind Point 1" );
        private readonly ParsedShort BindPoint2 = new( "Bind Point 2" );
        private readonly ParsedShort BindPoint3 = new( "Bind Point 3" );
        private readonly ParsedShort BindPoint4 = new( "Bind Point 4" );
        private readonly TmbOffsetFloat3 Scale = new( "Scale" );
        private readonly TmbOffsetFloat3 Rotation = new( "Rotation" );
        private readonly TmbOffsetFloat3 Position = new( "Position" );
        private readonly TmbOffsetFloat4 RGBA = new( "RGBA" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );
        private readonly ParsedInt Unk3 = new( "Unknown 3" );

        public C012() : base() {
            Parsed = new() {
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

            Duration.Value = 30;
            BindPoint1.Value = 1;
            BindPoint2.Value = 0xFF;
            BindPoint3.Value = 2;
            BindPoint4.Value = 0xFF;
            Scale.Value = new( 1 );
            RGBA.Value = new( 1 );
        }

        public C012( TmbReader reader ) : base( reader ) {
            ReadHeader( reader );
            ReadParsed( reader );
        }

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            WriteParsed( writer );
        }

        public override void Draw( string id ) {
            DrawTime( id );
            DrawParsed( id );
        }
    }
}
