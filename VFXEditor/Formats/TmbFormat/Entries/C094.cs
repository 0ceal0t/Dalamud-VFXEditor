using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using VfxEditor.Formats.TmbFormat.Base;
using VfxEditor.Formats.TmbFormat.Utils;
using VfxEditor.Parsing;

namespace VfxEditor.Formats.TmbFormat.Entries {
    public class C094 : TmbEntry {
        public const string MAGIC = "C094";
        public const string DISPLAY_NAME = "Invisibility";
        public override string DisplayName => DISPLAY_NAME;
        public override string Magic => MAGIC;

        public override int Size => 0x20;
        public override int ExtraSize => ExtraData ? 0x14 : 0;

        private bool ExtraData = true;

        private readonly ParsedInt FadeTime = new( "Fade Time" );
        private readonly ParsedInt Unk1 = new( "Unknown 1" );
        private readonly ParsedFloat StartVisibility = new( "Start Visibility" );
        private readonly ParsedFloat EndVisibility = new( "End Visibility" );

        // these are parsed separately
        private readonly ParsedBool EnableFilter = new( "Enable Filter" );
        private readonly ParsedFlag<InvisibilityFilter> Filter = new( "Filter" );
        private readonly ParsedInt Unk6 = new( "Unknown 6" );
        private readonly ParsedInt Unk7 = new( "Unknown 7" );
        private readonly ParsedInt Unk8 = new( "Unknown 8" );

        // Unk2 = 1, Unk3 = 8 -> ExtraSize = 0x14

        public C094( TmbFile file ) : base( file ) { }

        public C094( TmbFile file, TmbReader reader ) : base( file, reader ) {
            ExtraData = reader.ReadAtOffset( ( binaryReader ) => {
                EnableFilter.Read( binaryReader );
                Filter.Read( binaryReader );
                Unk6.Read( binaryReader );
                Unk7.Read( binaryReader );
                Unk8.Read( binaryReader );
            } );
        }

        protected override List<ParsedBase> GetParsed() => [
            FadeTime,
            Unk1,
            StartVisibility,
            EndVisibility
        ];

        public override void Write( TmbWriter writer ) {
            base.Write( writer );

            writer.WriteExtra( ( binaryWriter ) => {
                EnableFilter.Write( binaryWriter );
                Filter.Write( binaryWriter );
                Unk6.Write( binaryWriter );
                Unk7.Write( binaryWriter );
                Unk8.Write( binaryWriter );
            }, ExtraData );
        }

        public override void DrawBody() {
            base.DrawBody();

            ImGui.Checkbox( "Extra Data", ref ExtraData );
            if( ExtraData ) {
                EnableFilter.Draw();

                using( var disabled = ImRaii.Disabled( !EnableFilter.Value ) )
                using( var indent = ImRaii.PushIndent() ) {
                    Filter.Draw();
                }

                Unk6.Draw();
                Unk7.Draw();
                Unk8.Draw();
            }
        }
    }
}
