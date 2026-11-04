using System.IO;
using VfxEditor.Formats.AvfxFormat.Nodes;
using VfxEditor.Formats.AvfxFormat.Views.Nodes;

namespace VfxEditor.Formats.AvfxFormat.Binder {
    public class UiBinderView : AvfxNodeDropdownView<AvfxBinder> {
        public UiBinderView( AvfxFile file, NodeGroup<AvfxBinder> group ) : base( file, group, "Binder", true, true, "default_binder.vfxedit" ) { }

        public override void OnSelect( AvfxBinder item ) { }

        public override AvfxBinder Read( BinaryReader reader, int size ) {
            var item = new AvfxBinder( File ); // never has dependencies
            item.Read( reader, size );
            return item;
        }
    }
}