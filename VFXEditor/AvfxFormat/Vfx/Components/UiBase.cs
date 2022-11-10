using ImGuiNET;
using System.Collections.Generic;
using VfxEditor.AVFXLib;

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

        public static bool DrawCommandButton( AVFXBase assignable, string name, string id ) {
            if( !assignable.IsAssigned() ) {
                if( ImGui.SmallButton( $"+ {name}{id}" ) ) CommandManager.Avfx.Add( new UiAssignableCommand( assignable, true ) );
                return true;
            }

            return false;
        }

        public static void DrawCommandContextMenu( AVFXBase assignable, string name, string id ) {
            if( DrawUnassignContextMenu( id, name ) ) CommandManager.Avfx.Add( new UiAssignableCommand( assignable, false ) );
        }

        public static bool DrawCommandButton( List<AVFXBase> assignable, string name, string id ) {
            if( !assignable[0].IsAssigned() ) {
                if( ImGui.SmallButton( $"+ {name}{id}" ) ) CommandManager.Avfx.Add( new UiAssignableCommandMultiple( assignable, true ) );
                return true;
            }

            return false;
        }

        public static void DrawCommandContextMenu( List<AVFXBase> assignable, string name, string id ) {
            if( DrawUnassignContextMenu( id, name ) ) CommandManager.Avfx.Add( new UiAssignableCommandMultiple( assignable, false ) );
        }
    }
}
