using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class AvfxDisplaySplitView<T> : UiGenericSplitView where T : AvfxItem {
        public readonly List<T> Items;
        private T Selected = null;

        public AvfxDisplaySplitView( List<T> items ) : base( false, false ) {
            Items = items;
        }

        public override void DrawControls( string parentId ) { }

        public override void DrawLeftCol( string parentId ) {
            foreach( var item in Items.Where( x => x.IsAssigned() ) ) {
                if( ImGui.Selectable( item.GetText() + parentId, Selected == item ) ) {
                    Selected = item;
                }
            }

            // not assigned, can be added
            foreach( var item in Items.Where( x => !x.IsAssigned() ) ) item.Draw( parentId );
        }

        public override void DrawRightCol( string parentId ) {
            if( Selected != null && Selected.IsAssigned() ) Selected.Draw( parentId );
        }
    }
}
