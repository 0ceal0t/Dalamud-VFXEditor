using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class UiEmitterView : UiNodeDropdownView<AvfxEmitter> {
        public UiEmitterView( AvfxFile file, UiNodeGroup<AvfxEmitter> group ) : base( file, group, "Emitter", true, true, "default_emitter.vfxedit" ) { }

        public override void OnSelect( AvfxEmitter item ) { }
    }
}