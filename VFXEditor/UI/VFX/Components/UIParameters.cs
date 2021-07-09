using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX {
    public class UIParameters : UIItem {
        public string Name;

        public UIParameters( string name) {
            Assigned = true;
            Name = name;
        }

        public void Add(UIBase item ) {
            Attributes.Add( item );
        }

        public override void DrawBody( string parentId ) {
            DrawAttrs( parentId );
        }

        public override string GetDefaultText() {
            return Name;
        }
    }
}
