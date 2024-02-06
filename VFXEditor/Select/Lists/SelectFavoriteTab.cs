using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.Utils;

namespace VfxEditor.Select.Lists {
    public class SelectFavoriteTab : SelectListTab {
        private SelectResult DraggingItem;

        public SelectFavoriteTab( SelectDialog dialog, string name, List<SelectResult> items ) : base( dialog, name, items ) { }

        protected override bool PostRow( SelectResult item, int idx ) {
            if( UiUtils.DrawDragDrop( Items, item, item?.DisplayString ?? string.Empty, ref DraggingItem, $"{Name}-FAVORITE", false ) ) {
                Plugin.Configuration.Save();
                return true;
            }

            if( base.PostRow( item, idx ) ) return true;

            // ======== POPUP ============

            using var _ = ImRaii.PushId( idx );

            if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ImGui.OpenPopup( "FavoritePopup" );

            using var style = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, DefaultWindowPadding );
            if( ImGui.BeginPopup( "FavoritePopup" ) ) {
                if( ImGui.InputText( "##Rename", ref item.DisplayString, 128, ImGuiInputTextFlags.AutoSelectAll ) ) Plugin.Configuration.Save();
                Dialog.PlayPopupItems( item.Path );
                ImGui.EndPopup();
            }

            return false;
        }
    }
}
