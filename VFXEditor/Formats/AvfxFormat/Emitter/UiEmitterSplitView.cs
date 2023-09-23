using System.Collections.Generic;

namespace VfxEditor.AvfxFormat {
    public class UiEmitterSplitView : AvfxItemSplitView<AvfxEmitterItem> {
        public readonly AvfxEmitter Emitter;
        public readonly bool IsParticle;

        public UiEmitterSplitView( string id, List<AvfxEmitterItem> items, AvfxEmitter emitter, bool isParticle ) : base( id, items ) {
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

        public override AvfxEmitterItem CreateNewAvfx() => new( IsParticle, Emitter, true );
    }
}
