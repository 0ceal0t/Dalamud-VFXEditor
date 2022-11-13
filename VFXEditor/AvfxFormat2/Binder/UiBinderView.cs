using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class UiBinderView : UiNodeDropdownView<AvfxBinder> {
        public UiBinderView( AvfxFile file, UiNodeGroup<AvfxBinder> group ) : base( file, group, "Binder", true, true, "default_binder.vfxedit" ) { }

        public override void OnSelect( AvfxBinder item ) { }
    }
}