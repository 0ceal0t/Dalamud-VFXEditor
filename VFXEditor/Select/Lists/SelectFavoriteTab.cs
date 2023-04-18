using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.Select.Lists {
    public class SelectFavoriteTab : SelectListTab, IDraggableList<SelectResult> {
        private SelectResult DraggingItem;

        public SelectFavoriteTab( SelectDialog dialog, string name, List<SelectResult> items ) : base( dialog, name, items ) { }

        protected override bool PostRow( SelectResult item, string id, int idx ) {
            if( IDraggableList<SelectResult>.DrawDragDrop( this, item, $"{id}-FAVORITE" ) ) {
                Plugin.Configuration.Save();
                return true;
            }

            if( base.PostRow( item, id, idx ) ) return true;

            var itemId = $"{id}{idx}";
            if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ImGui.OpenPopup( $"{itemId}-context" );

            // Make sure to remove and then restore window padding if necessary
            ImGui.PopStyleVar( 1 );
            if( ImGui.BeginPopup( $"{itemId}-context" ) ) {
                if( ImGui.Selectable( $"Rename{itemId}" ) ) {
                    
                }
                ImGui.EndPopup();
            }
            ImGui.PushStyleVar( ImGuiStyleVar.WindowPadding, new Vector2( 0, 2 ) );

            return false;
        }

        // For drag+drop

        public SelectResult GetDraggingItem() => DraggingItem;

        public void SetDraggingItem( SelectResult item ) => DraggingItem = item;

        public List<SelectResult> GetItems() => Items;

        public string GetDraggingText( SelectResult item ) => item?.DisplayString ?? string.Empty;
    }
}
