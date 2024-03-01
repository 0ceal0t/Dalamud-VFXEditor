using Dalamud.Interface;
using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.Spawn;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public enum VfxVisibility {
        Unknown_0 = 0,
        Unknown_1 = 1,
        Everyone = 2,
        Unknown_3 = 3,
    }

    public class C012 : TmbEntry {
        public const string MAGIC = "C012";
        public const string DISPLAY_NAME = "VFX";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x48;
        public override int ExtraSize => 4 * ( 3 + 3 + 3 + 4 );

        private readonly ParsedInt Duration = new( "Duration", value: 30 );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly TmbOffsetString Path = new( "Path", new() {
            new() {
                Icon = () => VfxSpawn.IsActive ? FontAwesomeIcon.Times : FontAwesomeIcon.Eye,
                Remove = false,
                Action = ( string path ) => {
                    if( VfxSpawn.IsActive ) VfxSpawn.Remove();
                    else VfxSpawn.OnSelf( path, false );
                }
            }
        }, false );
        private readonly ParsedShort BindPoint1 = new( "Bind Point 1", value: 1 );
        private readonly ParsedShort BindPoint2 = new( "Bind Point 2", value: 0xFF );
        private readonly ParsedShort BindPoint3 = new( "Bind Point 3", value: 2 );
        private readonly ParsedShort BindPoint4 = new( "Bind Point 4", value: 0xFF );
        private readonly TmbOffsetFloat3 Scale = new( "Scale", defaultValue: new( 1 ) );
        private readonly TmbOffsetAngle3 Rotation = new( "Rotation" );
        private readonly TmbOffsetFloat3 Position = new( "Position" );
        private readonly TmbOffsetFloat4 RGBA = new( "RGBA", defaultValue: new( 1 ) );
        private readonly ParsedEnum<VfxVisibility> Visibility = new( "Visibility" );
        private readonly ParsedInt Unk3 = new( "Unknown 3" );

        public C012( TmbFile file ) : base( file ) { }

        public C012( TmbFile file, TmbReader reader ) : base( file, reader ) { }

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
            Visibility,
            Unk3
        };
    }
}
