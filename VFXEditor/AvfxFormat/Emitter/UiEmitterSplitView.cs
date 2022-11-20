using Dalamud.Logging;
using System;
using System.Collections.Generic;

namespace VfxEditor.AvfxFormat {
    public class UiEmitterSplitView : UiItemSplitView<AvfxEmitterItem> {
        public readonly AvfxEmitter Emitter;
        public readonly bool IsParticle;

        public UiEmitterSplitView( List<AvfxEmitterItem> items, AvfxEmitter emitter, bool isParticle ) : base( items ) {
            Emitter = emitter;
            IsParticle = isParticle;
        }

        public override void Disable( AvfxEmitterItem item ) {
            if( IsParticle ) item.ParticleSelect.Disable();
            else item.EmitterSelect.Disable();
        }

        public override void Enable( AvfxEmitterItem item ) {
            if( IsParticle ) item.ParticleSelect.Enable();
            else item.EmitterSelect.Enable();
        }

        public override AvfxEmitterItem CreateNewAvfx() => new AvfxEmitterItem( IsParticle, Emitter, true );
    }
}
