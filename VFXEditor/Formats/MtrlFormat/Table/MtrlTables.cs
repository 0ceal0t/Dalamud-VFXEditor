using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.MtrlFormat.Table.Color;

namespace VfxEditor.Formats.MtrlFormat.Table {
    public enum ColorTableSize : int {
        Legacy = 16 * 32,
        Extended = 32 * 64
    }

    public enum DyeTableSize : int {
        Legacy = 16 * 2,
        Extended = 32 * 4
    }

    public class MtrlTables {
        public readonly MtrlFile File;
        public readonly List<MtrlColorTableRow> Rows = [];
        public readonly MtrlColorTableSplitView RowView;

        public bool Legacy { get; private set; } = true; // default
        public int Count => Legacy ? 16 : 32;

        private MtrlStain Stain;

        public MtrlTables( MtrlFile file ) {
            File = file;
            RowView = new( this );
            for( var i = 0; i < 32; i++ ) Rows.Add( new( this ) );
        }

        public MtrlTables( MtrlFile file, BinaryReader reader, long dataEnd ) : this( file ) {
            var size = ( int )( dataEnd - reader.BaseStream.Position );
            if( size < ( int )ColorTableSize.Legacy ) return;
            Legacy = !( size >= ( int )ColorTableSize.Extended );

            for( var i = 0; i < Count; i++ ) Rows[i].Read( reader );

            // Read dye rows
            size = ( int )( dataEnd - reader.BaseStream.Position );
            if( !file.DyeTableEnabled || size < ( Legacy ? ( int )DyeTableSize.Legacy : ( int )DyeTableSize.Extended ) ) return;

            for( var i = 0; i < Count; i++ ) Rows[i].DyeRow.Read( reader );
        }

        public void Write( BinaryWriter writer ) {
            if( File.ColorTableEnabled ) for( var i = 0; i < Count; i++ ) Rows[i].Write( writer );
            if( File.DyeTableEnabled ) for( var i = 0; i < Count; i++ ) Rows[i].DyeRow.Write( writer );
        }

        public void Draw() {
            var legacy = Legacy;
            if( ImGui.Checkbox( "Legacy", ref legacy ) ) Legacy = legacy;
            ImGui.SameLine();

            DrawDyeCombo();

            ImGui.Separator();
            RowView.Draw();
        }

        private void DrawDyeCombo() {
            var v = Stain == null ? new( 0 ) : Stain.Color;
            ImGui.ColorEdit3( "##Color", ref v, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.DisplayRGB | ImGuiColorEditFlags.InputRGB | ImGuiColorEditFlags.NoTooltip );
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                ImGui.SameLine();
            }

            ImGui.SetNextItemWidth( 200f );
            using var combo = ImRaii.Combo( "Preview Dye", Stain == null ? "[NONE]" : Stain.Name );
            if( !combo ) return;

            DrawDyeComboRow( null, 0 );
            foreach( var (item, idx) in Plugin.MtrlManager.Stains.WithIndex() ) DrawDyeComboRow( item, idx + 1 );
        }

        private void DrawDyeComboRow( MtrlStain stain, int idx ) {
            using var _ = ImRaii.PushId( idx );
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );

            var v = stain == null ? new( 0 ) : stain.Color;
            ImGui.ColorEdit3( "##Color", ref v, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.DisplayRGB | ImGuiColorEditFlags.InputRGB | ImGuiColorEditFlags.NoTooltip );

            ImGui.SameLine();
            if( ImGui.Selectable( stain == null ? "[NONE]" : stain.Name, Stain == stain ) ) {
                Stain = stain;
                foreach( var item in Rows ) item.SetPreviewStain( stain );
                RowView.GetSelected()?.RefreshPreview();
            }

            if( Stain == stain ) ImGui.SetItemDefaultFocus();
        }
    }
}
