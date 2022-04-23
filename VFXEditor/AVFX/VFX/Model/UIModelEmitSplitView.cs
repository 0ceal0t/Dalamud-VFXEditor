using System.Collections.Generic;

namespace VFXEditor.AVFX.VFX {
    public class UIModelEmitSplitView : UIItemSplitView<UIModelEmitterVertex> {
        public UIModel Model;

        public UIModelEmitSplitView( List<UIModelEmitterVertex> items, UIModel model ) : base( items, true, true ) {
            Model = model;
        }

        public override UIModelEmitterVertex OnNew() {
            var vnum = Model.Model.VNums.Add();
            var emit = Model.Model.EmitVertexes.Add();
            return new UIModelEmitterVertex( vnum, emit, Model );
        }

        public override void OnDelete( UIModelEmitterVertex item ) {
            Model.Model.EmitVertexes.Remove( item.Vertex );
            Model.Model.VNums.Remove( item.VertNumber );
        }
    }
}
