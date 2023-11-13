using ImGuiNET;
using OtterGui;
using OtterGui.Raii;
using System.Collections.Generic;
using System.IO;

namespace VfxEditor.Formats.MtrlFormat.Table {
    public class MtrlColorTable {
        public const int Size = 16 * MtrlColorTableRow.Size;

        public readonly List<MtrlColorTableRow> Rows = new();
        private readonly MtrlColorTableSplitView RowView;
        private MtrlDye PreviewDye;

        public MtrlColorTable( MtrlFile file ) {
            for( var i = 0; i < 16; i++ ) Rows.Add( new( file ) );
            RowView = new( Rows );
        }

        public MtrlColorTable( MtrlFile file, BinaryReader reader ) {
            for( var i = 0; i < 16; i++ ) Rows.Add( new( file, reader ) );
            RowView = new( Rows );
        }

        public void Draw() {
            DrawDyeCombo();
            ImGui.Separator();
            RowView.Draw();
        }

        private void DrawDyeCombo() {
            var v = PreviewDye == null ? new( 0 ) : PreviewDye.Color;
            ImGui.ColorEdit3( "##Color", ref v, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.DisplayRGB | ImGuiColorEditFlags.InputRGB | ImGuiColorEditFlags.NoTooltip );
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                ImGui.SameLine();
            }

            ImGui.SetNextItemWidth( 300f );
            using var combo = ImRaii.Combo( "Preview Dye", PreviewDye == null ? "[NONE]" : PreviewDye.Name );
            if( !combo ) return;

            DrawDyeComboRow( null, 0 );
            foreach( var (item, idx) in Plugin.MtrlManager.Dyes.WithIndex() ) DrawDyeComboRow( item, idx + 1 );
        }

        private void DrawDyeComboRow( MtrlDye dye, int idx ) {
            using var _ = ImRaii.PushId( idx );
            using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );

            var v = dye == null ? new( 0 ) : dye.Color;
            ImGui.ColorEdit3( "##Color", ref v, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.DisplayRGB | ImGuiColorEditFlags.InputRGB | ImGuiColorEditFlags.NoTooltip );

            ImGui.SameLine();
            if( ImGui.Selectable( dye == null ? "[NONE]" : dye.Name, PreviewDye == dye ) ) {
                PreviewDye = dye;
                foreach( var item in Rows ) item.SetPreviewDye( dye );
                Plugin.DirectXManager.MaterialPreview.RefreshColorRow();
            }

            if( PreviewDye == dye ) ImGui.SetItemDefaultFocus();
        }

        public void Write( BinaryWriter writer ) => Rows.ForEach( x => x.Write( writer ) );
    }
}
