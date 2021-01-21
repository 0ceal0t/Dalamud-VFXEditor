using AVFXLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using AVFXLib.AVFX;

namespace VFXEditor.UI.VFX
{
    public class UIEmitterView : UIDropdownView<UIEmitter> {
        public UIEmitterView( AVFXBase avfx) : base(avfx, "##EMIT", "Select an Emitter" ) {
            List<UIEmitter> items = AVFX.Emitters.Select( item => new UIEmitter( item, this ) ).ToList();
            UINode._Emitters = new UINodeGroup<UIEmitter>( items );
            Group = UINode._Emitters;
        }

        public override UIEmitter OnNew() {
            return new UIEmitter( AVFX.addEmitter(), this );
        }
        public override void OnDelete( UIEmitter item ) {
            AVFX.removeEmitter( item.Emitter );
        }
        public override byte[] OnExport( UIEmitter item ) {
            return item.Emitter.toAVFX().toBytes();
        }
        public override UIEmitter OnImport( AVFXNode node ) {
            AVFXEmitter item = new AVFXEmitter();
            item.read( node );
            AVFX.addEmitter( item );
            return new UIEmitter( item, this );
        }
    }
}
