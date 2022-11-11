using System.Collections.Generic;
using VfxEditor.AVFXLib.Model;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiModelEmitSplitView : UiItemSplitView<UiModelEmitterVertex> {
        public UiModel Model;

        public UiModelEmitSplitView( List<UiModelEmitterVertex> items, UiModel model ) : base( items ) {
            Model = model;
        }

        public override void RemoveFromAvfx( UiModelEmitterVertex item ) {
            Model.Model.EmitVertexes.SetAssigned( true );
            Model.Model.VertexNumbers.SetAssigned( true );
            Model.Model.EmitVertexes.EmitVertexes.Remove( item.Vertex );
            Model.Model.VertexNumbers.VertexNumbers.Remove( item.VertexNumber );
        }

        public override void AddToAvfx( UiModelEmitterVertex item, int idx ) {
            Model.Model.EmitVertexes.SetAssigned( true );
            Model.Model.VertexNumbers.SetAssigned( true );
            Model.Model.EmitVertexes.EmitVertexes.Insert( idx, item.Vertex );
            Model.Model.VertexNumbers.VertexNumbers.Insert( idx, item.VertexNumber );
        }

        public override UiModelEmitterVertex CreateNewAvfx() => new( new AVFXVertexNumber( 0 ), new AVFXEmitVertex(), Model );
    }
}
