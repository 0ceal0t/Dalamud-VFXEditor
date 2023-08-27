using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using System.Numerics;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Select.Lists {
    public class SelectFavoriteTab : SelectListTab, IDraggableList<SelectResult> {
        private SelectResult DraggingItem;

        public SelectFavoriteTab( SelectDialog dialog, string name, List<SelectResult> items ) : base( dialog, name, items ) { }

        protected override bool PostRow( SelectResult item, int idx ) {
            if( IDraggableList<SelectResult>.DrawDragDrop( this, item, $"{Name}-FAVORITE" ) ) {
                Plugin.Configuration.Save();
                return true;
            }
            if( base.PostRow( item, idx ) ) return true;

            using var _ = ImRaii.PushId( idx );

            if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ImGui.OpenPopup( "RenameFavorite" );

            using var style = ImRaii.PushStyle( ImGuiStyleVar.WindowPadding, new Vector2( 4, 6 ) );
            if( ImGui.BeginPopup( "RenameFavorite" ) ) {
                if( ImGui.InputText( "##Rename", ref item.DisplayString, 128, ImGuiInputTextFlags.AutoSelectAll ) ) Plugin.Configuration.Save();
                ImGui.EndPopup();
            }

            return false;
        }

        // For drag+drop

        public SelectResult GetDraggingItem() => DraggingItem;

        public void SetDraggingItem( SelectResult item ) => DraggingItem = item;

        public List<SelectResult> GetItems() => Items;

        public string GetDraggingText( SelectResult item ) => item?.DisplayString ?? string.Empty;
    }
}
