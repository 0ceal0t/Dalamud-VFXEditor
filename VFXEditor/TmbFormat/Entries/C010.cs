using ImGuiNET;
using OtterGui.Raii;
using System;
using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    [Flags]
    public enum AnimationFlags {
        TimeControlEnabled = 0x01,
        Unknown_2 = 0x02,
        Unknown_3 = 0x04,
        Unknown_4 = 0x08,
        Unknown_5 = 0x10,
        Unknown_6 = 0x20,
        Unknown_7 = 0x40,
        Unknown_8 = 0x80
    }

    public class C010 : TmbEntry {
        public const string MAGIC = "C010";
        public const string DISPLAY_NAME = "Animation (C010)";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x28;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration", defaultValue: 50 );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedFlag<AnimationFlags> Flags = new( "Flags" );
        private readonly ParsedFloat AnimationStart = new( "Animation Start Frame" );
        private readonly ParsedFloat AnimationEnd = new( "Animation End Frame" );
        private readonly TmbOffsetString Path = new( "Path", maxSize: 31 );
        private readonly ParsedInt Unk5 = new( "Unknown 1" );

        public C010( bool papEmbedded ) : base( papEmbedded ) { }

        public C010( TmbReader reader, bool papEmbedded ) : base( reader, papEmbedded ) {
            ReadHeader( reader );
            ReadParsed( reader );
        }

        protected override List<ParsedBase> GetParsed() => new() {
            Duration,
            Unk1,
            Flags,
            AnimationStart,
            AnimationEnd,
            Path,
            Unk5
        };

        public override void Draw() {
            DrawHeader();

            Flags.Draw( Command );

            using( var disabled = ImRaii.Disabled( !Flags.HasFlag( AnimationFlags.TimeControlEnabled ) ) ) {
                Duration.Draw( Command );
                AnimationStart.Draw( Command );
                AnimationEnd.Draw( Command );
            }

            Unk1.Draw( Command );
            Path.Draw( Command );
            Unk5.Draw( Command );
        }
    }
}
