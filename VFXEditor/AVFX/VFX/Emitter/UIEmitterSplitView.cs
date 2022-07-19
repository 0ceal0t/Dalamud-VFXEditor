using System.Collections.Generic;

namespace VFXEditor.AVFX.VFX {
    public class UIEmitterSplitView : UIItemSplitView<UIEmitterItem> {
        public readonly UIEmitter Emitter;
        public readonly bool IsParticle;

        public UIEmitterSplitView( List<UIEmitterItem> items, UIEmitter emitter, bool isParticle ) : base( items, true, true ) {
            Emitter = emitter;
            IsParticle = isParticle;
        }

        public override UIEmitterItem OnNew() {
            if( IsParticle ) {
                return new UIEmitterItem( Emitter.Emitter.AddParticle(), true, Emitter );
            }
            else {
                return new UIEmitterItem( Emitter.Emitter.AddEmitter(), false, Emitter );
            }
        }

        public override void OnDelete( UIEmitterItem item ) {
            if( IsParticle ) {
                Emitter.Emitter.RemoveParticle( item.Iteration );
                item.ParticleSelect.DeleteSelect();
            }
            else {
                Emitter.Emitter.RemoveEmitter( item.Iteration );
                item.EmitterSelect.DeleteSelect();
            }
        }
    }
}
