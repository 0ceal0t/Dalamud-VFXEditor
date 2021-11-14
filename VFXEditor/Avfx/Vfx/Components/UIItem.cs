using ImGuiNET;
using System.Collections.Generic;

namespace VFXEditor.Avfx.Vfx {
    public abstract class UIItem : UIBase {
        public int Idx;
        public List<UIBase> Attributes = new();

        public UIItem() { }

        public override void Init() {
            base.Init();
            Attributes = new List<UIBase>();
        }

        public virtual string GetText() {
            return GetDefaultText();
        }
        public abstract string GetDefaultText();

        public abstract void DrawBody( string parentId );
        public virtual void DrawUnAssigned( string parentId ) { }
        public override void Draw( string parentId ) { }

        public void DrawAttrs( string parentId ) {
            DrawList( Attributes, parentId );
        }

        public static void DrawListTabs( List<UIItem> items, string parentId ) {
            var idx = 0;
            foreach( var item in items ) {
                if( !item.Assigned ) {
                    if(idx > 0 ) {
                        ImGui.SameLine();
                    }
                    item.Draw( parentId );
                    idx++;
                }
            }

            ImGui.BeginTabBar( parentId + "-Tabs" );
            foreach( var item in items ) {
                if( !item.Assigned ) continue;
                if(ImGui.BeginTabItem( item.GetText() + parentId + "-Tabs" ) ) {
                    item.DrawBody( parentId );
                    ImGui.EndTabItem();
                }
            }
            ImGui.EndTabBar();
        }
    }
}
