using ImGuiNET;
using System.Collections.Generic;

namespace VFXEditor.AVFX.VFX {
    public abstract class UIItem : UIBase {
        public int Idx;

        public abstract bool IsAssigned();

        public virtual string GetText() => GetDefaultText();

        public abstract string GetDefaultText();

        public abstract void DrawBody( string parentId );

        public virtual void DrawUnAssigned( string parentId ) { }

        public override void Draw( string parentId ) { }

        public static void DrawListTabs( List<UIItem> items, string parentId ) {
            var numerOfUnassigned = 0;
            foreach( var item in items ) { // Draw unassigned
                if( item.IsAssigned() ) continue;

                if( numerOfUnassigned > 0 ) ImGui.SameLine();
                item.Draw( parentId );
                numerOfUnassigned++;
            }

            ImGui.BeginTabBar( parentId + "-Tabs" ); // Draw assigned
            foreach( var item in items ) {
                if( !item.IsAssigned() ) continue;

                if( ImGui.BeginTabItem( item.GetText() + parentId + "-Tabs" ) ) {
                    item.DrawBody( parentId );
                    ImGui.EndTabItem();
                }
            }
            ImGui.EndTabBar();
        }
    }
}
