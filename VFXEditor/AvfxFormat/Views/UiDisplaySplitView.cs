using Dalamud.Interface;
using ImGuiNET;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Utils;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiDisplaySplitView<T> : UiGenericSplitView where T : UiItem {
        public readonly List<T> Items;
        private T Selected = null;

        public UiDisplaySplitView( List<T> items ) : base( false, false ) {
            Items = items;
        }

        public override void DrawControls( string parentId ) { }

        public override void DrawLeftCol( string parentId ) {
            foreach( var item in Items.Where( UiItem.IsAssigned ) ) {
                if( ImGui.Selectable( item.GetText() + parentId, Selected == item ) ) {
                    Selected = item;
                }
            }

            // not assigned, can be added
            foreach( var item in Items.Where( UiItem.IsUnassigned ) ) {
                item.DrawInline( parentId );
            }
        }

        public override void DrawRightCol( string parentId ) {
            if( Selected != null && UiItem.IsAssigned( Selected ) ) {
                Selected.DrawInline( parentId );
            }
        }
    }
}
