using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Ui.Components {
    public class SimpleSplitView<T> : SplitView<T>, IDraggableList<T> where T : class, IUiItem {
        protected readonly string ItemName;
        protected readonly bool AllowReorder;

        private T DraggingItem;

        public SimpleSplitView( string itemName, List<T> items, bool allowNew, bool allowReorder ) : base( items, allowNew ) {
            ItemName = itemName;
            AllowReorder = allowReorder;
        }

        protected override void DrawControls( string id ) {
            ImGui.PushFont( UiBuilder.IconFont );
            if( ImGui.Button( $"{( char )FontAwesomeIcon.Plus}{id}" ) ) OnNew();
            if( Selected != null ) {
                ImGui.SameLine();
                if( UiUtils.RemoveButton( $"{( char )FontAwesomeIcon.Trash}{id}" ) ) {
                    OnDelete( Selected );
                    Selected = null;
                }
            }
            ImGui.PopFont();
        }

        protected virtual void OnNew() { }

        protected virtual void OnDelete( T item ) { }

        protected override bool DrawLeftItem( T item, int idx, string id ) {
            if( ImGui.Selectable( $"{GetText( item, idx )}{id}{idx}", item == Selected ) ) Selected = item;

            if( AllowReorder && IDraggableList<T>.DrawDragDrop( this, item, $"{id}-SPLIT" ) ) return true;

            return false;
        }

        protected virtual string GetText( T item, int idx ) => $"{ItemName} {idx}";

        protected override void DrawSelected( string id ) => Selected.Draw( $"{id}{Items.IndexOf( Selected )}" );

        // For drag+drop

        public T GetDraggingItem() => DraggingItem;

        public void SetDraggingItem( T item ) => DraggingItem = item;

        public List<T> GetItems() => Items;

        public string GetDraggingText( T item ) => GetText( item, Items.IndexOf( item ) );
    }
}
