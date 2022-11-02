using ImGuiNET;
using System.Collections.Generic;

namespace VfxEditor.AvfxFormat.Vfx {
    public interface IUiBase {
        public abstract void DrawInline( string parentId );

        public static void DrawList( List<IUiBase> items, string parentId ) {
            foreach( var item in items ) { // Draw assigned items
                if( item is UiAssignableItem optionalItem && !optionalItem.IsAssigned() ) continue;
                item.DrawInline( parentId );
            }

            foreach( var item in items ) { // Draw unassigned items
                if( item is UiAssignableItem optionalItem && !optionalItem.IsAssigned() ) item.DrawInline( parentId );
            }
        }

        public static bool DrawUnassignContextMenu( string id, string name ) {
            var ret = false;
            if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) {
                ImGui.OpenPopup( $"Unassign-{id}-{name}" );
            }
            if( ImGui.BeginPopup( $"Unassign-{id}-{name}" ) ) {
                if( ImGui.Selectable( $"Unassign {name}" ) ) {
                    ret = true;
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }
            return ret;
        }
    }
}
