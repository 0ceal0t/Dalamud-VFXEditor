using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
using System.Collections.Generic;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Ui.Components {
    public class GenericSplitView<T> : SplitView<T>, IDraggableList<T> where T : class, IUiItem {
        protected readonly bool AllowReorder;

        private T DraggingItem;

        public GenericSplitView( string id, List<T> items, bool allowNew, bool allowReorder ) : base( id, items, allowNew ) {
            AllowReorder = allowReorder;
        }

        protected override void DrawControls() {
            using var font = ImRaii.PushFont( UiBuilder.IconFont );

            if( ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) OnNew();
            if( Selected != null ) {
                ImGui.SameLine();
                if( UiUtils.RemoveButton( FontAwesomeIcon.Trash.ToIconString() ) ) {
                    OnDelete( Selected );
                    Selected = null;
                }
            }
        }

        protected virtual void OnNew() { }

        protected virtual void OnDelete( T item ) { }

        protected override bool DrawLeftItem( T item, int idx ) {
            using( var _ = ImRaii.PushId( idx ) ) {
                if( ImGui.Selectable( GetText( item, idx ), item == Selected ) ) Selected = item;
            }

            if( AllowReorder && IDraggableList<T>.DrawDragDrop( this, item, $"{Id}-SPLIT" ) ) return true;
            return false;
        }

        protected virtual string GetText( T item, int idx ) => $"{Id} {idx}";

        protected override void DrawSelected() {
            using var _ = ImRaii.PushId( Items.IndexOf( Selected ) );
            Selected.Draw();
        }


        // For drag+drop

        public T GetDraggingItem() => DraggingItem;

        public void SetDraggingItem( T item ) => DraggingItem = item;

        public List<T> GetItems() => Items;

        public string GetDraggingText( T item ) => GetText( item, Items.IndexOf( item ) );
    }
}
