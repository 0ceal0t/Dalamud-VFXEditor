using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
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
            using var popup = ImRaii.Popup( "RecentPopup" );
            if( popup ) {
                if( UiUtils.IconSelectable( FontAwesomeIcon.Trash, "Delete" ) ) {
                    Items.Remove( item );
                    Plugin.Configuration.Save();
                    return true;
                }
                if( item.Type != SelectResultType.Local ) Dialog.PlayPopupItems( item.Path );
            }

            return false;
        }
    }
}
