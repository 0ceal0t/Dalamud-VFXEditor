using Dalamud.Interface;
using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C063 : TmbEntry {
        public const string MAGIC = "C063";
        public const string DISPLAY_NAME = "Sound";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x20;
        public override int ExtraSize => 0;

        private readonly ParsedInt Loop = new( "Loop", value: 1 );
        private readonly ParsedInt Interrupt = new( "Interrupt" );
        private readonly TmbOffsetString Path = new( "Path", null, true );
        private readonly ParsedInt SoundIndex = new( "Sound Index" );
        private readonly ParsedInt SoundPosition = new( "Sound Position", value: 1 );

        public C063( TmbFile file ) : base( file ) {
            SetupIcon();
        }

        public C063( TmbFile file, TmbReader reader ) : base( file, reader ) {
            SetupIcon();
        }

        private void SetupIcon() {
            Path.Icons.Insert( 0, new() {
                Icon = () => FontAwesomeIcon.VolumeUp,
                Remove = false,
                Action = ( string value ) => Plugin.ResourceLoader.PlaySound( value, SoundIndex.Value )
            } );
        }

        protected override List<ParsedBase> GetParsed() => [
            Loop,
            Interrupt,
            Path,
            SoundIndex,
            SoundPosition
        ];
    }
}
