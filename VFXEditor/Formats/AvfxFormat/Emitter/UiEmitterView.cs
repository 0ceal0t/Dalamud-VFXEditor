using System.IO;

namespace VfxEditor.AvfxFormat {
    public class UiEmitterView : AvfxNodeDropdownView<AvfxEmitter> {
        public UiEmitterView( AvfxFile file, NodeGroup<AvfxEmitter> group ) : base( file, group, "Emitter", true, true, "default_emitter.vfxedit" ) { }

        public override void OnSelect( AvfxEmitter item ) { }

        public override AvfxEmitter Read( BinaryReader reader, int size ) {
            var item = new AvfxEmitter( File, File.NodeGroupSet );
            item.Read( reader, size );
            return item;
        }
    }
}