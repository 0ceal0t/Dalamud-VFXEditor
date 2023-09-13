using System.Collections.Generic;

namespace VfxEditor.AvfxFormat {
    public class UiUvSetSplitView : UiItemSplitView<AvfxParticleUvSet> {
        public UiUvSetSplitView( List<AvfxParticleUvSet> items ) : base( "UVSet", items ) { }

        protected override void DrawControls() {
            ShowControls = Items.Count < 4; // only allow up to 4 items
            base.DrawControls();
        }

        public override void Disable( AvfxParticleUvSet item ) { }

        public override void Enable( AvfxParticleUvSet item ) { }

        public override AvfxParticleUvSet CreateNewAvfx() => new();
    }
}
