using System.Collections.Generic;
using VfxEditor.Formats.AvfxFormat.Views.Items;

namespace VfxEditor.Formats.AvfxFormat.Particle.UvSet {
    public class UiUvSetSplitView : AvfxItemSplitView<AvfxParticleUvSet> {
        public UiUvSetSplitView( List<AvfxParticleUvSet> items ) : base( "UVSet", items ) { }

        protected override void DrawControls() {
            AllowNew = Items.Count < 4; // only allow up to 4 items
            base.DrawControls();
        }

        public override AvfxParticleUvSet CreateNewAvfx() => new();
    }
}
