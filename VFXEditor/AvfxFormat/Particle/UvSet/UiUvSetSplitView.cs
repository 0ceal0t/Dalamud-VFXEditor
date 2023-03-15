using System;
using System.Collections.Generic;

namespace VfxEditor.AvfxFormat {
    public class UiUvSetSplitView : UiItemSplitView<AvfxParticleUvSet> {
        public UiUvSetSplitView( List<AvfxParticleUvSet> items ) : base( items ) { }

        protected override void DrawControls( string id ) {
            AllowNew = Items.Count < 4; // only allow up to 4 items
            base.DrawControls( id );
        }

        public override void Disable( AvfxParticleUvSet item ) { }

        public override void Enable( AvfxParticleUvSet item ) { }

        public override AvfxParticleUvSet CreateNewAvfx() => new();
    }
}
