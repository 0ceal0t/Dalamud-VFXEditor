using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.Vfx {
    public class UIParameterTabs : UIItem {
        public string Name;
        public List<UIItem> Items = new();

        public UIParameterTabs( string name) {
            Assigned = true;
            Name = name;
        }

        public void Add(UIItem item ) {
            Items.Add( item );
        }

        public override void DrawBody( string parentId ) {
            DrawListTabs( Items, parentId );
        }

        public override string GetDefaultText() {
            return Name;
        }
    }
}
