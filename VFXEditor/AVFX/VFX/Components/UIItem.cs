using ImGuiNET;
using System.Collections.Generic;

namespace VFXEditor.AVFX.VFX {
    public abstract class UIItem : IUIBase {
        public int Idx;

        public abstract string GetDefaultText();
        public abstract void DrawInline( string parentId );
        public virtual string GetText() => GetDefaultText();

        public static bool IsAssigned( UIItem item ) => !IsUnassigned( item );
        public static bool IsUnassigned( UIItem item ) => item is UIAssignableItem optionalItem && !optionalItem.IsAssigned();

        public static void DrawListTabs( List<UIItem> items, string parentId ) {
            var numerOfUnassigned = 0;
            foreach( var item in items ) { // Draw unassigned
                if( item is not UIAssignableItem optionalItem || optionalItem.IsAssigned() ) continue;

                if( numerOfUnassigned > 0 ) ImGui.SameLine();
                item.DrawInline( parentId );
                numerOfUnassigned++;
            }

            ImGui.BeginTabBar( parentId + "-Tabs" ); // Draw assigned
            foreach( var item in items ) {
                if( item is UIAssignableItem optionalItem && !optionalItem.IsAssigned() ) continue;

                if( ImGui.BeginTabItem( item.GetText() + parentId + "-Tabs" ) ) {
                    item.DrawInline( parentId );
                    ImGui.EndTabItem();
                }
            }
            ImGui.EndTabBar();
        }
    }
}
