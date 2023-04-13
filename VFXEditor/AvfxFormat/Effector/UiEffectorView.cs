using System;
using System.IO;
using VfxEditor.Ui.Nodes;

namespace VfxEditor.AvfxFormat {
    public class UiEffectorView : UiNodeDropdownView<AvfxEffector> {
        public UiEffectorView( AvfxFile file, NodeGroup<AvfxEffector> group ) : base( file, group, "Effector", true, true, "default_effector.vfxedit" ) { }

        public override void OnSelect( AvfxEffector item ) { }

        public override AvfxEffector Read( BinaryReader reader, int size ) {
            var item = new AvfxEffector();
            item.Read( reader, size );
            return item;
        }
    }
}