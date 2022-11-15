using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat {
    public class UiUvSetSplitView : UiItemSplitView<AvfxParticleUvSet> {
        public UiUvSetSplitView( List<AvfxParticleUvSet> items ) : base( items ) { }

        public override void DrawControls( string parentId ) {
            AllowNew = Items.Count < 4; // only allow up to 4 items
            base.DrawControls( parentId );
        }

        public override void Disable( AvfxParticleUvSet item ) { }

        public override void Enable( AvfxParticleUvSet item ) { }

        public override AvfxParticleUvSet CreateNewAvfx() => new AvfxParticleUvSet();
    }
}
