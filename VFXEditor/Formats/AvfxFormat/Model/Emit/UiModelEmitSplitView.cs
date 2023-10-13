using System.Collections.Generic;

namespace VfxEditor.AvfxFormat {
    public class UiModelEmitSplitView : AvfxItemSplitView<UiEmitVertex> {
        private readonly AvfxModel Model;

        public UiModelEmitSplitView( AvfxModel model, List<UiEmitVertex> items ) : base( "EmitVertex", items ) {
            Model = model;
        }

        public override void OnChange() => Model.RefreshModelPreview();

        public override UiEmitVertex CreateNewAvfx() => new( Model, new AvfxEmitVertex(), new AvfxVertexNumber() );
    }
}
