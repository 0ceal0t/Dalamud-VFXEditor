using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Parsing;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.TmbFormat {
    public class TmfcData : IUiItem {
        public readonly ParsedUInt Unk1 = new( "Unknown 1" );
        public readonly ParsedUInt Unk2 = new( "Unknown 2", size: 1 );
        public readonly ParsedUInt Unk3 = new( "Unknown 3", size: 1 );
        public readonly ParsedUInt Unk4 = new( "Unknown 4", size: 1 );
        public readonly ParsedUInt Unk5 = new( "Unknown 5", size: 1 );
        public readonly ParsedUInt Unk6 = new( "Unknown 6", size: 1 );
        public readonly ParsedUInt Unk7 = new( "Unknown 7", size: 1 );
        public readonly ParsedUInt Unk8 = new( "Unknown 8", size: 1 );
        public readonly ParsedUInt Unk9 = new( "Unknown 9", size: 1 );
        private readonly uint TempRowCount = 0;

        public readonly List<ParsedBase> Parsed = new();

        public readonly List<TmfcRow> Rows = new();

        public int Size => 0x10 + ( 0x18 * Rows.Count );

        public readonly bool PapEmbedded;
        public CommandManager Command => PapEmbedded ? CommandManager.Pap : CommandManager.Tmb;

        // 24 bytes x count

        public TmfcData( BinaryReader reader, bool papEmbedded ) {
            PapEmbedded = papEmbedded;
            Parsed.AddRange( new ParsedBase[] {
                Unk1,
                Unk2,
                Unk3,
                Unk4,
                Unk5,
                Unk6,
                Unk7,
                Unk8,
                Unk9
            } );

            Parsed.ForEach( x => x.Read( reader ) );
            TempRowCount = reader.ReadUInt32();
        }

        public void ReadRows( BinaryReader reader ) {
            for( var i = 0; i < TempRowCount; i++ ) Rows.Add( new TmfcRow( reader ) );
        }

        public void Write( BinaryWriter writer ) {
            Parsed.ForEach( x => x.Write( writer ) );
            writer.Write( Rows.Count );
        }

        public void WriteRows( BinaryWriter writer ) => Rows.ForEach( x => x.Write( writer ) );

        public void Draw() {
            Parsed.ForEach( x => x.Draw( Command ) );

            for( var idx = 0; idx < Rows.Count; idx++ ) {
                if( ImGui.CollapsingHeader( $"Row {idx}", ImGuiTreeNodeFlags.DefaultOpen ) ) {
                    using var _ = ImRaii.PushId( idx );
                    using var indent = ImRaii.PushIndent();
                    Rows[idx].Draw( Command );
                }
            }
        }
    }
}
