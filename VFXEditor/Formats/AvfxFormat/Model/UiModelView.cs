using System.IO;

namespace VfxEditor.AvfxFormat {
    public class UiModelView : AvfxNodeSplitView<AvfxModel> {
        public UiModelView( AvfxFile file, NodeGroup<AvfxModel> group ) : base( file, group, "Model", true, true, "default_model.vfxedit2" ) { }

        public override void OnSelect( AvfxModel item ) { }

        public override AvfxModel Read( BinaryReader reader, int size ) {
            var item = new AvfxModel( File );
            item.Read( reader, size );
            return item;
        }
    }
}