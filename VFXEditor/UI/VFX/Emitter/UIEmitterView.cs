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
        public UIEmitterView( UIMain main, AVFXBase avfx ) : base( main, avfx, "##EMIT", "Select an Emitter", defaultPath: "emitter_default.vfxedit" ) {
            Group = main.Emitters;
            Group.Items = AVFX.Emitters.Select( item => new UIEmitter( Main, item ) ).ToList();
        }

        public override void OnDelete( UIEmitter item ) {
            AVFX.RemoveEmitter( item.Emitter );
        }
        public override byte[] OnExport( UIEmitter item ) {
            return item.Emitter.ToAVFX().ToBytes();
        }
        public override UIEmitter OnImport( AVFXNode node, bool has_dependencies = false ) {
            AVFXEmitter item = new AVFXEmitter();
            item.Read( node );
            AVFX.AddEmitter( item );
            return new UIEmitter( Main, item, has_dependencies );
        }
    }
}
