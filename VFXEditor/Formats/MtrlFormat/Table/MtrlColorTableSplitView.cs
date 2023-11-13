using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.Formats.MtrlFormat.Table {
    public class MtrlColorTableSplitView : UiSplitView<MtrlColorTableRow> {
        public MtrlColorTableSplitView( List<MtrlColorTableRow> items ) : base( "Row", items, false, false ) { }

        protected override bool DrawLeftItem( MtrlColorTableRow item, int idx ) {
            using var _ = ImRaii.PushId( idx );
            using( var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing ) ) {
                item.Diffuse.DrawPreview();
                ImGui.SameLine();
                item.Specular.DrawPreview();
                ImGui.SameLine();
                item.Emissive.DrawPreview();
                ImGui.SameLine();
            }

            if( ImGui.Selectable( $"Row {idx}", item == Selected ) ) Selected = item;

            return false;
        }
    }
}