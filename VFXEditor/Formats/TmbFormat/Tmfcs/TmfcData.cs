using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Data.Command;
using VfxEditor.Parsing;
using VfxEditor.Parsing.Int;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Formats.TmbFormat.Tmfcs {
    public class TmfcData : IUiItem {
        public const int HeaderSize = 0x10;

        public readonly TmbFile File;

        public readonly ParsedUInt Unk1 = new( "Unknown 1" );
        public readonly ParsedUInt Unk2 = new( "Unknown 2", size: 1 );
        public readonly ParsedUInt Unk3 = new( "Unknown 3", size: 1 );
        public readonly ParsedUInt Unk4 = new( "Unknown 4", size: 1 );
        public readonly ParsedSByte Unk5 = new( "Unknown 5" );

        private readonly long HeaderPosition;
        private readonly int TempRowOffset;
        private readonly uint TempRowCount;

        public readonly List<ParsedBase> Parsed = [];

        public readonly List<TmfcRow> Rows = [];

        public int Size => HeaderSize + ( TmfcRow.Size * Rows.Count );

        public CommandManager Command => File.Command;

        public TmfcData( BinaryReader reader, TmbFile file ) {
            File = file;
            Parsed.AddRange( [
                Unk1,
                Unk2,
                Unk3,
                Unk4,
                Unk5,
            ] );

            HeaderPosition = reader.BaseStream.Position;
            Parsed.ForEach( x => x.Read( reader ) );
            TempRowOffset = reader.ReadInt32();
            TempRowCount = reader.ReadUInt32();
        }

        public void ReadRows( BinaryReader reader ) {
            if( TempRowCount == 0 ) return;
            reader.BaseStream.Position = HeaderPosition + TempRowOffset;
            for( var i = 0; i < TempRowCount; i++ ) Rows.Add( new TmfcRow( reader ) );
        }

        public void Write( BinaryWriter writer, int rowOffset ) {
            Parsed.ForEach( x => x.Write( writer ) );
            writer.Write( rowOffset );
            writer.Write( Rows.Count );
        }

        public void WriteRows( BinaryWriter writer ) {
            for( var idx = 0; idx < Rows.Count; idx++ ) {
                Rows[idx].Write( writer, idx + 1 < Rows.Count ? Rows[idx + 1] : null );
            }
        }

        public void Draw() {
            Parsed.ForEach( x => x.Draw() );

            for( var idx = 0; idx < Rows.Count; idx++ ) {
                if( ImGui.CollapsingHeader( $"Row {idx}", ImGuiTreeNodeFlags.DefaultOpen ) ) {
                    using var _ = ImRaii.PushId( idx );
                    using var indent = ImRaii.PushIndent();
                    Rows[idx].Draw();
                }
            }
        }
    }
}