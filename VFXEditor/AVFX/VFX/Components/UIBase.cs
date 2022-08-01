using ImGuiNET;
using System.Collections.Generic;

namespace VFXEditor.AVFX.VFX {
    public abstract class UIBase {
        public abstract void Draw( string parentId );

        public static void DrawList( List<UIBase> items, string parentId ) {
            foreach( var item in items ) { // Draw assigned items
                if( item is UIItem optionalItem && !optionalItem.IsAssigned() ) continue;
                item.Draw( parentId );
            }

            foreach( var item in items ) { // Draw unassigned items
                if( item is UIItem optionalItem && !optionalItem.IsAssigned() ) item.Draw( parentId );
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
