using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Ui.Components.SplitViews;

namespace VfxEditor.AvfxFormat {
    public class AvfxDisplaySplitView<T> : ItemSplitView<T> where T : AvfxItem {
        public AvfxDisplaySplitView( string id, List<T> items ) : base( id, items ) { }

        protected override void DrawLeftColumn() {
            var idx = 0;
            foreach( var item in Items.Where( x => x.IsAssigned() ) ) {
                using( var _ = ImRaii.PushId( idx ) ) {
                    if( item is AvfxOptional assignable ) assignable.AssignedCopyPaste( assignable.GetDefaultText() );
                    if( ImGui.Selectable( item.GetText(), Selected == item ) ) {
                        Selected = item;
                    }
                }
                idx++;
            }

            // not assigned, can be added
            foreach( var item in Items.Where( x => !x.IsAssigned() ) ) item.Draw();
        }

        protected override bool DrawLeftItem( T item, int idx ) => false;

        protected override void DrawSelected() {
            if( Selected.IsAssigned() ) Selected.Draw();
        }
    }
}
