using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class UiEffectorView : UiNodeDropdownView<AvfxEffector> {
        public UiEffectorView( AvfxFile file, UiNodeGroup<AvfxEffector> group ) : base( file, group, "Effector", true, true, "default_effector.vfxedit" ) { }

        public override void OnSelect( AvfxEffector item ) { }
    }
}