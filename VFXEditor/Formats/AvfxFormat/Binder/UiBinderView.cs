using System.IO;
using VfxEditor.Ui.Nodes;

namespace VfxEditor.AvfxFormat {
    public class UiBinderView : UiNodeDropdownView<AvfxBinder> {
        public UiBinderView( AvfxFile file, NodeGroup<AvfxBinder> group ) : base( file, group, "Binder", true, true, "default_binder.vfxedit" ) { }

        public override void OnSelect( AvfxBinder item ) { }

        public override AvfxBinder Read( BinaryReader reader, int size ) {
            var item = new AvfxBinder(); // never has dependencies
            item.Read( reader, size );
            return item;
        }
    }
}