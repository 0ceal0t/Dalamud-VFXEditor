using System.Collections.Generic;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiEmitterSplitView : UiItemSplitView<UiEmitterItem> {
        public readonly UiEmitter Emitter;
        public readonly bool IsParticle;

        public UiEmitterSplitView( List<UiEmitterItem> items, UiEmitter emitter, bool isParticle ) : base( items, true, true ) {
            Emitter = emitter;
            IsParticle = isParticle;
        }

        public override UiEmitterItem OnNew() {
            if( IsParticle ) {
                return new UiEmitterItem( Emitter.Emitter.AddParticle(), true, Emitter );
            }
            else {
                return new UiEmitterItem( Emitter.Emitter.AddEmitter(), false, Emitter );
            }
        }

        public override void OnDelete( UiEmitterItem item ) {
            if( IsParticle ) {
                Emitter.Emitter.RemoveParticle( item.Iteration );
                item.ParticleSelect.Disable();
            }
            else {
                Emitter.Emitter.RemoveEmitter( item.Iteration );
                item.EmitterSelect.Disable();
            }
        }
    }
}
