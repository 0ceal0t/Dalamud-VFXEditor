using ImGuiNET;
using System.Collections.Generic;

namespace VfxEditor.AvfxFormat.Vfx {
    public abstract class UiItem : IUiBase {
        public int Idx;

        public abstract string GetDefaultText();
        public abstract void DrawInline( string parentId );
        public virtual string GetText() => GetDefaultText();

        public static bool IsAssigned( UiItem item ) => !IsUnassigned( item );
        public static bool IsUnassigned( UiItem item ) => item is UiAssignableItem optionalItem && !optionalItem.IsAssigned();

        public static void DrawListTabs( List<UiItem> items, string parentId ) {
            var numerOfUnassigned = 0;
            foreach( var item in items ) { // Draw unassigned
                if( item is not UiAssignableItem optionalItem || optionalItem.IsAssigned() ) continue;

                if( numerOfUnassigned > 0 ) ImGui.SameLine();
                item.DrawInline( parentId );
                numerOfUnassigned++;
            }

            ImGui.BeginTabBar( parentId + "-Tabs" ); // Draw assigned
            foreach( var item in items ) {
                if( item is UiAssignableItem optionalItem && !optionalItem.IsAssigned() ) continue;

                if( ImGui.BeginTabItem( item.GetText() + parentId + "-Tabs" ) ) {
                    item.DrawInline( parentId );
                    ImGui.EndTabItem();
                }
            }
            ImGui.EndTabBar();
        }
    }
}
