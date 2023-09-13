using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.SklbFormat.Mapping {
    public unsafe class SklbSimpleMappingSplitView : CommandSplitView<SklbSimpleMapping> {
        private readonly SklbMapping Mapping;

        public SklbSimpleMappingSplitView( SklbMapping mapping, List<SklbSimpleMapping> items ) :
            base( "Mapping", items, false, null, () => new( mapping ), () => CommandManager.Sklb ) {
            Mapping = mapping;
        }

        protected override bool DrawLeftItem( SklbSimpleMapping item, int idx ) {
            using var _ = ImRaii.PushId( idx );

            if( ImGui.Selectable( item.BoneA.GetText( Mapping.SkeletonA ), item == Selected, ImGuiSelectableFlags.SpanAllColumns ) ) {
                Selected = item;
            }

            ImGui.SameLine();
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                ImGui.Text( FontAwesomeIcon.CaretRight.ToIconString() );
            }

            ImGui.SameLine();
            ImGui.Text( item.BoneB.GetText( Mapping.SkeletonB ) );

            return false;
        }
    }
}
