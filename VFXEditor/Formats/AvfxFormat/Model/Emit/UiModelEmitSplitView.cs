using System.Collections.Generic;

namespace VfxEditor.AvfxFormat {
    public class UiModelEmitSplitView : UiItemSplitView<UiEmitVertex> {
        public UiModelEmitSplitView( List<UiEmitVertex> items ) : base( "EmitVertex", items ) { }

        public override void Disable( UiEmitVertex item ) { }

        public override void Enable( UiEmitVertex item ) { }

        public override UiEmitVertex CreateNewAvfx() => new( new AvfxEmitVertex(), new AvfxVertexNumber() );
    }
}
