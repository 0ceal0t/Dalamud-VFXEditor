using ImGuiNET;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml.Linq;
using VfxEditor.AVFXLib;
using VfxEditor.Utils;

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

        public static bool DrawAddButton( AVFXBase assignable, string name, string id ) {
            if( !assignable.IsAssigned() ) {
                if( ImGui.SmallButton( $"+ {name}{id}" ) ) CommandManager.Avfx.Add( new UiAssignableCommand( assignable, true ) );
                return true;
            }
            return false;
        }

        public static void DrawRemoveContextMenu( AVFXBase assignable, string name, string id ) {
            if( DrawUnassignContextMenu( id, name ) ) CommandManager.Avfx.Add( new UiAssignableCommand( assignable, false ) );
        }

        public static bool DrawAddButton( List<AVFXBase> assignable, string name, string id ) {
            if( !assignable[0].IsAssigned() ) {
                if( ImGui.SmallButton( $"+ {name}{id}" ) ) CommandManager.Avfx.Add( new UiAssignableCommandMultiple( assignable, true ) );
                return true;
            }
            return false;
        }

        public static void DrawRemoveContextMenu( List<AVFXBase> assignable, string name, string id ) {
            if( DrawUnassignContextMenu( id, name ) ) CommandManager.Avfx.Add( new UiAssignableCommandMultiple( assignable, false ) );
        }

        public static bool DrawAddButtonRecurse( AVFXBase assignable, string name, string id ) {
            if( !assignable.IsAssigned() ) {
                if( ImGui.SmallButton( $"+ {name}{id}" ) ) CommandManager.Avfx.Add( new UiAssignableCommandRecurse( assignable, true ) );
                return true;
            }
            return false;
        }

        public static void DrawRemoveContextMenuRecurse( AVFXBase assignable, string name, string id ) {
            if( DrawUnassignContextMenu( id, name ) ) CommandManager.Avfx.Add( new UiAssignableCommandRecurse( assignable, false ) );
        }

        public static bool DrawRemoveButton( AVFXBase assignable, string name, string id ) {
            if( UiUtils.RemoveButton( $"Delete {name}{id}", small: true ) ) {
                CommandManager.Avfx.Add( new UiAssignableCommand( assignable, false ) );
                return true;
            }
            return false;
        }
    }
}
