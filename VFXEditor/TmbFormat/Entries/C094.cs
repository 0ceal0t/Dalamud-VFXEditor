using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.Parsing;
using VfxEditor.TmbFormat.Utils;

namespace VfxEditor.TmbFormat.Entries {
    [Flags]
    public enum InvisibilityFilter {
        Character = 0x01,
        MainWeapon = 0x02,
        OffhandWeapon = 0x04,
        Summon = 0x08,
        Unknown_1 = 0x10,
        Unknown_2 = 0x20,
        Unknown_3 = 0x40,
        Unknown_4 = 0x80,
    }

    public class C094 : TmbEntry {
        public const string MAGIC = "C094";
        public const string DISPLAY_NAME = "Invisibility (C094)";
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
        private readonly ParsedInt Unk4 = new( "Unknown 4" );
        private readonly ParsedInt Unk5 = new( "Unknown 5" );
        private readonly ParsedInt Unk6 = new( "Unknown 6" );

        // Unk2 = 1, Unk3 = 8 -> ExtraSize = 0x14

        public C094( bool papEmbedded ) : base( papEmbedded ) { }

        public C094( TmbReader reader, bool papEmbedded ) : base( reader, papEmbedded ) {
            ReadHeader( reader );
            ReadParsed( reader );

            ExtraData = reader.ReadAtOffset( ( binaryReader ) => {
                EnableFilter.Read( binaryReader );
                Filter.Read( binaryReader );
                Unk4.Read( binaryReader );
                Unk5.Read( binaryReader );
                Unk6.Read( binaryReader );
            } );
        }

        protected override List<ParsedBase> GetParsed() => new() {
            FadeTime,
            Unk1,
            StartVisibility,
            EndVisibility
        };

        public override void Write( TmbWriter writer ) {
            WriteHeader( writer );
            WriteParsed( writer );

            writer.WriteExtra( ( binaryWriter ) => {
                EnableFilter.Write( binaryWriter );
                Filter.Write( binaryWriter );
                Unk4.Write( binaryWriter );
                Unk5.Write( binaryWriter );
                Unk6.Write( binaryWriter );
            }, ExtraData );
        }

        public override void Draw( string id ) {
            DrawHeader( id );
            DrawParsed( id );

            ImGui.Checkbox( $"Extra Data{id}", ref ExtraData );
            if( ExtraData ) {
                EnableFilter.Draw( id, Command );

                var filtersEnabled = EnableFilter.Value == true;
                if( !filtersEnabled ) ImGui.BeginDisabled();
                ImGui.Indent();
                Filter.Draw( id, Command );
                ImGui.Unindent();
                if( !filtersEnabled ) ImGui.EndDisabled();

                Unk4.Draw( id, Command );
                Unk5.Draw( id, Command );
                Unk6.Draw( id, Command );
            }
        }
    }
}
