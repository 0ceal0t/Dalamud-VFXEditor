using System.Collections.Generic;
using VfxEditor.AVFXLib.Model;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiModelEmitSplitView : UiItemSplitView<UiModelEmitterVertex> {
        public UiModel Model;

        public UiModelEmitSplitView( List<UiModelEmitterVertex> items, UiModel model ) : base( items ) {
            Model = model;
        }

        public override UiModelEmitterVertex OnNew() {
            //var vnum = Model.Model.VertexNumbers.Add();
            //var emit = Model.Model.EmitVertexes.Add();
            //return new UiModelEmitterVertex( vnum, emit, Model );
            return null;
        }

        public override void RemoveFromAvfx( UiModelEmitterVertex item ) {
            Model.Model.EmitVertexes.EmitVertexes.Remove( item.Vertex );
            Model.Model.VertexNumbers.VertexNumbers.Remove( item.VertexNumber );
        }

        public override void AddToAvfx( UiModelEmitterVertex item, int idx ) {
            Model.Model.EmitVertexes.EmitVertexes.Insert( idx, item.Vertex );
            Model.Model.VertexNumbers.VertexNumbers.Insert( idx, item.VertexNumber );
        }
    }
}
