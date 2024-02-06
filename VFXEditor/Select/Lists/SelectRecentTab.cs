using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.Utils;

namespace VfxEditor.Select.Lists {
    public class SelectRecentTab : SelectListTab {
        public SelectRecentTab( SelectDialog dialog, string name, List<SelectResult> items ) : base( dialog, name, items ) { }

        protected override bool PostRow( SelectResult item, int idx ) {
            if( base.PostRow( item, idx ) ) return true;

            // ======== POPUP ============

            using var _ = ImRaii.PushId( idx );

            if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ImGui.OpenPopup( "RecentPopup" );

            using var style = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, DefaultWindowPadding );
            if( ImGui.BeginPopup( "RecentPopup" ) ) {
                if( UiUtils.IconSelectable( FontAwesomeIcon.Trash, "Delete" ) ) {
                    Items.Remove( item );
                    Plugin.Configuration.Save();
                    return true;
                }
                Dialog.PlayPopupItems( item.Path );
                ImGui.EndPopup();
            }

            return false;
        }
    }
}
