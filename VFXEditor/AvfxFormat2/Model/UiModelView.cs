using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class UiModelView : UiNodeSplitView<AvfxModel> {
        public UiModelView( AvfxFile file, UiNodeGroup<AvfxModel> group ) : base( file, group, "Model", true, true, "default_model.vfxedit2" ) { }

        public override void OnSelect( AvfxModel item ) => item.OnSelect();

        public override AvfxModel Read( BinaryReader reader, int size, bool hasDependencies ) {
            var item = new AvfxModel();
            item.Read( reader, size );
            return item;
        }
    }
}