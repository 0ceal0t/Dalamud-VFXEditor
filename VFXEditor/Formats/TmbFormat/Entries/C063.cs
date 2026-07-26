using Dalamud.Interface;
using System;
using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    [Flags]
    public enum SoundPositionFilter {
        Use_Min_Max_Range = 0x01,
        Stop_On_Animation_End = 0x02,
        Use_Bind_Id = 0x04,
    }
    public class C063 : TmbEntry {
        public const string MAGIC = "C063";
        public const string DISPLAY_NAME = "Sound";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x20;
        public override int ExtraSize => 0;

        private readonly ParsedInt Loop = new( "Loop/Duration", value: 1 );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly TmbOffsetString Path = new( "Path", null, true, "sound/replace_me.scd" );
        private readonly ParsedInt SoundIndex = new( "Sound Index" );
        private readonly ParsedFlag<SoundPositionFilter> SoundPosition = new( "Sound Position", size: 1 );
        private readonly ParsedByte BindId = new( "Bind Id" );
        private readonly ParsedShort Unk2 = new( "Unknown 2" );

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
                Action = value => Plugin.ResourceLoader.PlaySound( value, SoundIndex.Value )
            } );
        }

        protected override List<ParsedBase> GetParsed() => [
            Loop,
            Unk1,
            Path,
            SoundIndex,
            SoundPosition,
            BindId,
            Unk2
        ];
    }
}
