using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VfxEditor.AvfxFormat2 {
    public class UiEffectorView : UiNodeDropdownView<AvfxEffector> {
        public UiEffectorView( AvfxFile file, UiNodeGroup<AvfxEffector> group ) : base( file, group, "Effector", true, true, "default_effector.vfxedit" ) { }

        public override void OnSelect( AvfxEffector item ) { }

        public override AvfxEffector Read( BinaryReader reader, int size, bool hasDependencies ) {
            var item = new AvfxEffector( hasDependencies );
            item.Read( reader, size );
            return item;
        }
    }
}