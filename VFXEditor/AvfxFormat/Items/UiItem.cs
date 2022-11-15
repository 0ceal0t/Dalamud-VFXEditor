using ImGuiNET;
using System;
using System.Collections.Generic;

namespace VfxEditor.AvfxFormat {
    public interface IUiItem : IAvfxUiBase {
        public string GetDefaultText();
        public string GetText();

        public static void DrawListTabs<T>( List<T> items, string parentId ) where T : IUiItem {
            var numerOfUnassigned = 0;
            foreach( var item in items ) { // Draw unassigned
                if( item is not AvfxOptional optionalItem || optionalItem.IsAssigned() ) continue;

                if( numerOfUnassigned > 0 ) ImGui.SameLine();
                item.Draw( parentId );
                numerOfUnassigned++;
            }

            ImGui.BeginTabBar( parentId + "-Tabs" ); // Draw assigned
            foreach( var item in items ) {
                if( item is AvfxOptional optionalItem && !optionalItem.IsAssigned() ) continue;

                if( ImGui.BeginTabItem( item.GetText() + parentId + "-Tabs" ) ) {
                    item.Draw( parentId );
                    ImGui.EndTabItem();
                }
            }
            ImGui.EndTabBar();
        }
    }
}
