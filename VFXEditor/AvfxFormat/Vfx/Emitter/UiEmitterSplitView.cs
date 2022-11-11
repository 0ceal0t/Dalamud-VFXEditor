using System.Collections.Generic;
using VfxEditor.AVFXLib.Emitter;

namespace VfxEditor.AvfxFormat.Vfx {
    public class UiEmitterSplitView : UiItemSplitView<UiEmitterItem> {
        public readonly UiEmitter Emitter;
        public readonly bool IsParticle;

        public UiEmitterSplitView( List<UiEmitterItem> items, UiEmitter emitter, bool isParticle ) : base( items ) {
            Emitter = emitter;
            IsParticle = isParticle;
        }

        public override void RemoveFromAvfx( UiEmitterItem item ) {
            if( IsParticle ) {
                Emitter.Emitter.Particles.Remove( item.Iteration );
                item.ParticleSelect.Disable();
            }
            else {
                Emitter.Emitter.Emitters.Remove( item.Iteration );
                item.EmitterSelect.Disable();
            }
        }

        public override void AddToAvfx( UiEmitterItem item, int idx ) {
            if( IsParticle ) {
                Emitter.Emitter.Particles.Insert( idx, item.Iteration );
                item.ParticleSelect.Enable();
            }
            else {
                Emitter.Emitter.Emitters.Insert( idx, item.Iteration );
                item.EmitterSelect.Enable();
            }
        }

        public override UiEmitterItem CreateNewAvfx() => new( new AVFXEmitterItem(), IsParticle, Emitter );
    }
}
