using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using VfxEditor.Ui.Interfaces;
using VfxEditor.Utils;

namespace VfxEditor.Ui.Components.SplitViews {
    public class UiSplitView<T> : ItemSplitView<T> where T : class, IUiItem {
        protected readonly bool AllowReorder;
        protected T DraggingItem;

        public UiSplitView( string id, List<T> items, bool allowReorder ) : base( id, items ) {
            AllowReorder = allowReorder;
        }

        protected virtual bool RecordReorder() => false;

        protected override bool DrawLeftItem( T item, int idx ) {
            using( var _ = ImRaii.PushId( idx ) ) {
                if( ImGui.Selectable( GetText( item, idx ), item == Selected ) ) Selected = item;
            }

            if( AllowReorder && UiUtils.DrawDragDrop( Items, item, GetText( item, Items.IndexOf( item ) ), ref DraggingItem, $"{Id}-SPLIT", RecordReorder() ) ) return true;
            return false;
        }

        protected override void DrawSelected() {
            using var _ = ImRaii.PushId( Items.IndexOf( Selected ) );
            Selected.Draw();
        }
    }
}
