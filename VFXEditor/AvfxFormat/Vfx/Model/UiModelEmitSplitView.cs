using System.Collections.Generic;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiModelEmitSplitView : UiItemSplitView<UiModelEmitterVertex> {
        public UiModel Model;

        public UiModelEmitSplitView( List<UiModelEmitterVertex> items, UiModel model ) : base( items, true, true ) {
            Model = model;
        }

        public override UiModelEmitterVertex OnNew() {
            var vnum = Model.Model.VNums.Add();
            var emit = Model.Model.EmitVertexes.Add();
            return new UiModelEmitterVertex( vnum, emit, Model );
        }

        public override void OnDelete( UiModelEmitterVertex item ) {
            Model.Model.EmitVertexes.Remove( item.Vertex );
            Model.Model.VNums.Remove( item.VertNumber );
        }
    }
}
