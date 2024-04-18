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

    public abstract class MtrlTables {
        public readonly MtrlFile File;
        public readonly List<MtrlColorTableRow> Rows = [];
        public readonly MtrlColorTableSplitView RowView;

        private MtrlStain Stain;

        public MtrlTables( MtrlFile file ) {
            File = file;
            RowView = new( Rows );
        }

        public void Write( BinaryWriter writer ) {
            if( File.ColorTableEnabled ) foreach( var row in Rows ) row.Write( writer );
            if( File.DyeTableEnabled ) foreach( var row in Rows ) row.WriteDye( writer );
        }

        public void Draw() {
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

            ImGui.SetNextItemWidth( 300f );
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
