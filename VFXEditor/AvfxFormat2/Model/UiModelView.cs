using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class UiModelView : UiNodeSplitView<AvfxModel> {
        public UiModelView( AvfxFile file, UiNodeGroup<AvfxModel> group ) : base( file, group, "Model", true, true, "default_model.vfxedit2" ) { }

        public override void OnSelect( AvfxModel item ) => item.OnSelect();
    }
}