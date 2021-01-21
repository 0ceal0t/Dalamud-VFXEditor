using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.VFX {
    public class UIModelEmitSplitView : UIItemSplitView<UIModelEmitterVertex> {
        public UIModel Model;

        public UIModelEmitSplitView( List<UIModelEmitterVertex> items, UIModel model) : base( items, true, true ) {
            Model = model;
        }

        public override UIModelEmitterVertex OnNew() {
            var vnum = Model.Model.addVNum();
            var emit = Model.Model.addEmitVertex();
            return new UIModelEmitterVertex( vnum, emit, Model );
        }

        public override void OnDelete( UIModelEmitterVertex item ) {
            Model.Model.removeEmitVertex( item.Vertex );
            Model.Model.removeVNum( item.VertNumber );
        }
    }
}
