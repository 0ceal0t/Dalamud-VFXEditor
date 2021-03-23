using ImGuiNET;
using System.Collections.Generic;

namespace VFXEditor.UI.VFX {
    public abstract class UIItem : UIBase {
        public int Idx;
        public List<UIBase> Attributes = new List<UIBase>();

        public override void Init() {
            base.Init();
            Attributes = new List<UIBase>();
        }

        public abstract string GetText();
        public abstract void DrawBody( string parentId );
        public virtual void DrawUnAssigned( string parentId ) { }
        public override void Draw( string parentId ) { }

        public UIItem() {
        }

        public void DrawAttrs( string parentId ) {
            DrawList( Attributes, parentId );
        }
    }
}
