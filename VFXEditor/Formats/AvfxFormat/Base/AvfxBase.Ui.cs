using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using VfxEditor.Ui.Interfaces;

namespace VfxEditor.AvfxFormat {
    public partial class AvfxBase {
        private static bool UnassignPopup( string name ) {
            if( ImGui.IsItemClicked( ImGuiMouseButton.Right ) ) ImGui.OpenPopup( $"Unassign/{name}" );

            using var popup = ImRaii.Popup( $"Unassign/{name}" );
            if( popup ) {
                if( ImGui.Selectable( $"— {name.TrimStart( '#' )}" ) ) {
                    ImGui.CloseCurrentPopup();
                    return true;
                }
            }

            return false;
        }

        public static void DrawNamedItems<T>( List<T> items ) where T : INamedUiItem {
            SplitUnassigned( items, out var assigned, out var unassigned );

            if( unassigned.Count > 0 ) {
                using var style = ImRaii.PushStyle( ImGuiStyleVar.ItemSpacing, ImGui.GetStyle().ItemInnerSpacing );
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
                for( var idx = 0; idx < unassigned.Count; idx++ ) {
                    if( idx > 0 ) ImGui.SameLine();
                    unassigned[idx].Draw();
                }
                ImGui.SetCursorPosY( ImGui.GetCursorPosY() + 2 );
            }

            using var tabBar = ImRaii.TabBar( "Tabs" );
            if( !tabBar ) return;

            foreach( var item in assigned ) {
                using var tabBarItem = ImRaii.TabItem( $"{item.GetText()}" );
                if( tabBarItem ) item.Draw();
            }
        }

        public static void DrawItems<T>( List<T> items ) where T : IUiItem {
            SplitUnassigned( items, out var assigned, out var unassigned );

            assigned.ForEach( x => x.Draw() );
            unassigned.ForEach( x => x.Draw() );
        }

        public static void SplitUnassigned<T>( List<T> items, out List<T> assigned, out List<T> unassigned ) {
            assigned = [];
            unassigned = [];
            foreach( var item in items ) {
                if( item is AvfxOptional optionalItem && !optionalItem.IsAssigned() ) unassigned.Add( item );
                else assigned.Add( item );
            }
        }
    }
}
