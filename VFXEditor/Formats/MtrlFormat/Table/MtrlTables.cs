using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using System.IO;
using VfxEditor.Formats.MtrlFormat.Table.Color;

namespace VfxEditor.Formats.MtrlFormat.Table {
    public enum ColorTableSize : int {
        Standard = 16 * 32,
        Extended = 32 * 64
    }

    public enum DyeTableSize : int {
        Standard = 16 * 2,
        Extended = 32 * 4
    }

    public class MtrlTables {
        public readonly MtrlFile File;
        public readonly List<MtrlColorTableRow> Rows = [];
        public readonly MtrlColorTableSplitView RowView;

        public ColorTableSize Mode { get; private set; } = ColorTableSize.Standard; // default

        private MtrlStain Stain;

        public MtrlTables( MtrlFile file ) {
            File = file;
            RowView = new( this );
            for( var i = 0; i < 32; i++ ) Rows.Add( new( this ) );
        }

        public MtrlTables( MtrlFile file, BinaryReader reader, long dataEnd ) : this( file ) {
            var size = ( int )( dataEnd - reader.BaseStream.Position );
            int count;

            if( size >= ( int )ColorTableSize.Extended ) {
                Mode = ColorTableSize.Extended;
                count = 32;
            }
            else if( size >= ( int )ColorTableSize.Standard ) {
                Mode = ColorTableSize.Standard;
                count = 16; // only use 16
            }
            else return; // no need to read dye table

            for( var i = 0; i < count; i++ ) Rows[i].Read( reader );

            // Read dye rows
            size = ( int )( dataEnd - reader.BaseStream.Position );
            if( file.DyeTableEnabled && size >= ( int )DyeTableSize.Standard ) {
                for( var i = 0; i < count; i++ ) Rows[i].DyeRow.Read( reader );
            }
        }

        public void Write( BinaryWriter writer ) {
            var count = Mode == ColorTableSize.Standard ? 16 : 32;
            if( File.ColorTableEnabled ) for( var i = 0; i < count; i++ ) Rows[i].Write( writer );
            if( File.DyeTableEnabled ) for( var i = 0; i < count; i++ ) Rows[i].DyeRow.Write( writer );
        }

        public void Draw() {
            DrawModeCombo();
            ImGui.SameLine();
            DrawDyeCombo();

            ImGui.Separator();
            RowView.Draw();

            // TODO: mode select
        }

        private void DrawModeCombo() {
            ImGui.SetNextItemWidth( 200f );
            using var combo = ImRaii.Combo( "##Mode", Mode == ColorTableSize.Extended ? "Extended (Dawntrail)" : $"{Mode}" );
            if( !combo ) return;

            if( ImGui.Selectable( "Standard" ) ) Mode = ColorTableSize.Standard;
            if( ImGui.Selectable( "Extended (Dawntrail)" ) ) Mode = ColorTableSize.Extended;
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
