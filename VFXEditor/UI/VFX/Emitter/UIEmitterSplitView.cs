using AVFXLib.Models;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFXEditor.UI.Vfx
{
    public class UIEmitterSplitView : UIItemSplitView<UIEmitterItem>
    {
        public UIEmitter Emitter;
        public bool IsParticle;

        public UIEmitterSplitView( List<UIEmitterItem> items, UIEmitter emitter, bool isParticle ) : base( items, true, true )
        {
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
