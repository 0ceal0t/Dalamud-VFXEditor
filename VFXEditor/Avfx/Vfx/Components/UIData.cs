using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.Avfx.Vfx {
    public abstract class UIData : UIItem {
        public List<UIItem> Tabs = new();
        public UIItemSplitView<UIItem> SplitView;

        public UIData() {
            SplitView = new UIItemSplitView<UIItem>( Tabs );
        }

        public virtual void Dispose() { }

        public override string GetDefaultText() {
            return "Data";
        }

        public override void Draw( string parentId ) {
            DrawBody( parentId );
        }
        public override void DrawBody( string parentId ) {
            var id = parentId + "/Data";
            SplitView.Draw( id );
        }
    }
}
