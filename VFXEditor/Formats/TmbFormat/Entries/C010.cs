using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Base;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    public class C010 : TmbEntry {
        public const string MAGIC = "C010";
        public const string DISPLAY_NAME = "Animation";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x28;
        public override int ExtraSize => 0;

        private readonly ParsedInt Duration = new( "Duration" );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedBool TimeControl = new( "Use Time Control", size: 1 );
        private readonly ParsedFlag<C010Unk2Flags> Unk3 = new ( "Unknown 3", size: 2 );
        private readonly ParsedFlag<C010Unk3Flags> Unk4 = new ( "Unknown 4", size: 1 ); //cfxf
        private readonly ParsedFloat AnimationStart = new( "Animation Start Frame" );
        private readonly ParsedFloat AnimationEnd = new( "Animation End Frame" );
        private readonly TmbOffsetString Path = new( "Path" );
        private readonly ParsedInt Unk2 = new( "Unknown 2" );

        public C010( TmbFile file ) : base( file ) { }

        public C010( TmbFile file, TmbReader reader ) : base( file, reader ) { }

        protected override List<ParsedBase> GetParsed() => [
            Duration,
            Unk1,
            TimeControl,
            Unk3,
            Unk4,
            AnimationStart,
            AnimationEnd,
            Path,
            Unk2
        ];

        public override void DrawBody() {
            DrawHeader();
            Unk1.Draw();
            Path.Draw();
            Unk2.Draw();
            TimeControl.Draw();

            using( var disabled = ImRaii.Disabled( TimeControl.Value == false ) ) {
                Duration.Draw();
                AnimationStart.Draw();
                AnimationEnd.Draw();
            }

            //jank, but works for now
            ImGui.Columns( 2 );
            Unk3.Draw();
            ImGui.NextColumn();
            Unk4.Draw();
            ImGui.Columns( 1 );
        }
    }
}
